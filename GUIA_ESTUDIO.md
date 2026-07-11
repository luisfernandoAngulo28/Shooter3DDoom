# Guía de estudio — Parcial Shooter3DDoom

Formato del parcial: **Defensa oral (30 pts)** + **Reto en vivo / modificación del proyecto (20 pts)**, sobre las 5 funciones obligatorias (50 pts) + 1 opcional (10 pts) que ya están implementadas.

---

## 1. Munición y recarga (10 pts) — `Disparar.cs`

**Qué hace:** el arma tiene `balasMax` balas. Cada disparo resta una (`balasActuales--`). Si `balasActuales == 0` no se puede disparar. Con `R` se inicia una coroutine `Recargar()` que espera `tiempoRecarga` segundos y restaura `balasActuales = balasMax`. El HUD se actualiza llamando a `GameManager.instancia.ActualizarUIMunicion(...)`.

**Piezas clave:**
- `recargando` (bool) bloquea disparo y bloquea iniciar otra recarga mientras ya se está recargando.
- `IEnumerator Recargar()` + `yield return new WaitForSeconds(tiempoRecarga)` → coroutine, no bloquea el `Update()`.
- Condición de disparo: `Input.GetMouseButtonDown(0) && Time.time >= proximo && balasActuales > 0`.

**Posibles preguntas orales:**
- ¿Por qué usar una coroutine y no un `if` con timer manual? → Porque `WaitForSeconds` suspende la ejecución sin bloquear el hilo principal ni el resto del `Update()`; es la forma idiomática en Unity de esperar tiempo sin `Thread.Sleep`.
- ¿Qué pasa si presionas R con el cargador lleno? → No pasa nada, hay guard `balasActuales < balasMax`.
- ¿Qué pasa si disparas justo cuando termina la recarga? → `recargando` se pone en `false` al final de la coroutine antes de devolver control a `Update()`, así que el siguiente frame ya puede disparar.
- **Reto típico:** agregar munición reserva (ej. no recargar infinito, sino desde un pool total), o cambiar la recarga a "por bala" en vez de "cargador completo".

---

## 2. Enemigos (30 pts) — `EnemigoIA.cs`

**Qué hace:** usa `NavMeshAgent` para perseguir al jugador (`agente.SetDestination(jugador.position)`). Cuando la distancia al jugador es `<= rangoDisparo`, se detiene (`agente.isStopped = true`), rota para mirarlo, y dispara por raycast con cadencia propia, igual que el jugador.

**Piezas clave:**
- `[RequireComponent(typeof(NavMeshAgent))]` — garantiza el componente en el GameObject.
- Búsqueda automática del jugador por tag si no se asignó en el Inspector: `GameObject.FindGameObjectWithTag("Player")`.
- Dos estados simples (sin máquina de estados formal): perseguir vs. disparar, decididos por distancia.
- `MirarJugador()` calcula dirección ignorando el eje Y (`dir.y = 0f`) para no inclinar al enemigo hacia arriba/abajo.
- El daño se aplica igual que el jugador: `Physics.Raycast` desde el enemigo hacia el jugador, y `hit.collider.GetComponentInParent<Vida>().RecibirDano(dano)`, pero validando `v.esJugador` para no autolesionarse entre enemigos.

**Requisitos de escena (no en código):** `NavMeshSurface` horneado sobre el piso, prefab enemigo con `NavMeshAgent` + `Vida` (esJugador = false) + `EnemigoIA` + `AudioSource`.

**Posibles preguntas orales:**
- ¿Por qué `NavMeshAgent` y no mover el `Transform` directamente? → Pathfinding automático que esquiva paredes/obstáculos del laberinto; moverlo a mano requeriría implementar A* o steering behaviors propios.
- ¿Cómo evita el enemigo dispararse a sí mismo o a otro enemigo? → El chequeo `v.esJugador` en `Disparar()` filtra el `Vida` recibido.
- ¿Qué pasa si `rangoDisparo` es mayor que el `alcance` del raycast? → El enemigo se detendría a disparar pero el raycast podría no llegar; hay que mantener `alcance >= rangoDisparo` (bug potencial a explicar si preguntan).
- **Reto típico:** agregar un estado de "patrulla" cuando no ve al jugador, usar `NavMeshAgent.velocity` para animaciones, o line-of-sight con un segundo raycast (evitar que dispare a través de paredes si el rango es mayor al pasillo).

---

## 3. Contador de enemigos + victoria doble (5 pts) — `GameManager.cs` + `Meta.cs`

**Qué hace:** `GameManager` cuenta enemigos al iniciar (`FindObjectsByType<EnemigoIA>().Length`), actualiza el HUD, y decrementa en `EnemigoMuerto()` (llamado desde `Vida.Morir()`). `Meta.cs` es un trigger que, al detectar al jugador, llama `GameManager.instancia.JugadorEnMeta()`. La victoria solo se dispara si **ambas** condiciones son verdaderas: `jugadorEnMeta && enemigosRestantes <= 0`.

**Piezas clave:**
- `RevisarVictoria()` se llama desde dos lugares distintos (`EnemigoMuerto()` y `JugadorEnMeta()`) porque no se sabe cuál de las dos condiciones se cumple última.
- `Mathf.Max(0, enemigosRestantes - 1)` evita contadores negativos.
- `Meta.cs` requiere un Collider marcado `Is Trigger` y que el jugador tenga tag `Player`.

**Posibles preguntas orales:**
- ¿Por qué llamar `RevisarVictoria()` en dos métodos distintos? → No se puede saber de antemano si el jugador llega a la meta antes o después de matar al último enemigo; cualquiera de los dos eventos puede ser el que complete la condición.
- ¿Qué pasa si tocas la meta antes de matar a todos? → `jugadorEnMeta = true` queda guardado, pero `RevisarVictoria()` no activa el panel porque `enemigosRestantes > 0`; en cuanto muera el último enemigo, `EnemigoMuerto()` vuelve a llamar `RevisarVictoria()` y ahí sí gana.
- **Reto típico:** mostrar mensaje distinto si llegas a la meta sin haber matado a todos ("Elimina a todos los enemigos"), o agregar temporizador.

---

## 4. Menú de Game Over con reinicio (5 pts) — `GameManager.cs`

**Qué hace:** `Vida.Morir()` (si `esJugador`) llama `GameManager.instancia.GameOver()`, que activa `panelGameOver`, pausa el juego (`Time.timeScale = 0f`) y libera el cursor (`Cursor.lockState = CursorLockMode.None; Cursor.visible = true`). El botón "Reintentar" del panel está conectado (OnClick, en el Inspector) a `GameManager.Reintentar()`, que restaura `Time.timeScale = 1f` y recarga la escena actual con `SceneManager.LoadScene(SceneManager.GetActiveScene().name)`.

**Piezas clave:**
- `Time.timeScale = 0f` congela físicas, animaciones y cualquier `Update` que dependa de `Time.deltaTime`, pero **no** detiene `Update()` en sí (por eso el botón sigue respondiendo, ya que UI usa `Time.unscaledDeltaTime` internamente).
- Por qué liberar el cursor: sin esto, el jugador no puede hacer clic en el botón porque el cursor sigue bloqueado en el centro de pantalla (`CursorLockMode.Locked`, seteado en `PrimeraPersona.Start()`).

**Posibles preguntas orales:**
- ¿Por qué no simplemente recargar la escena directo al morir (como estaba antes, ver `pasos.md`)? → Requisito del parcial: dar feedback antes de reiniciar y permitir reintentar sin perder el estado de la sesión de aprendizaje del jugador.
- ¿Qué pasa con `Time.timeScale` si no lo restauras en `Reintentar()`? → La nueva escena cargaría ya pausada; por eso se resetea a `1f` antes del `LoadScene`.
- **Reto típico:** agregar botón "Salir" (`Application.Quit()`), mostrar estadísticas (enemigos eliminados, tiempo) en el panel de Game Over.

---

## 5. Feedback de daño (10 pts) — `GameManager.cs` (`FlashDano`) + `Vida.cs`

**Qué hace:** cuando `Vida.RecibirDano()` se ejecuta sobre el jugador, llama `GameManager.instancia.FlashDano()`. Esto sube el alpha de una `Image` roja full-screen del Canvas a `alphaFlash`, y una coroutine la desvanece de vuelta a 0 con `Mathf.Lerp` a lo largo de `duracionFlash` segundos.

**Piezas clave:**
- `flashRoutine` guarda la referencia de la coroutine activa; si llega otro golpe antes de que termine el fade, `StopCoroutine(flashRoutine)` cancela la anterior para reiniciar el flash desde el máximo alpha (evita que dos flashes se pisen dejando alpha bajo).
- `Mathf.Lerp(alphaFlash, 0f, t / duracionFlash)` interpola el alpha en función del tiempo transcurrido normalizado.
- Nota: los enemigos tienen su propio feedback de daño distinto (flash de color en su material, `Vida.FlashDanoEnemigo()`), no el mismo sistema que el jugador — vale la pena diferenciarlo si preguntan.

**Posibles preguntas orales:**
- ¿Por qué guardar la referencia a la coroutine en vez de solo llamar `StartCoroutine` cada vez? → Para poder cancelar una coroutine de fade previa y evitar que compitan dos coroutines bajando el mismo alpha a distinta velocidad (glitch visual).
- ¿Por qué está en `GameManager` y no en `Vida.cs` directamente? → `Vida.cs` es genérico (sirve para jugador y enemigos), y la UI de daño en pantalla es un concepto de HUD/jugador, no de la lógica de vida en sí — separación de responsabilidades.
- **Reto típico:** cambiar la intensidad del flash según el daño recibido (`alphaFlash` proporcional a `cantidad`), o agregar un shake de cámara.

---

## Opcional: Botiquín (10 pts) — `Botiquin.cs` + `Vida.Curar()`

**Qué hace:** trigger que detecta al jugador (`GetComponent<Vida>()` con `esJugador == true`), llama `vida.Curar(curacion)` (con tope en `vidaMax` vía `Mathf.Min`), reproduce sonido con `AudioSource.PlayClipAtPoint` (no `PlayOneShot`, porque el objeto se destruye inmediatamente después y un `AudioSource` destruido cortaría el sonido) y se destruye con `Destroy(gameObject)`.

**Posibles preguntas orales:**
- ¿Por qué `PlayClipAtPoint` y no `PlayOneShot` en un `AudioSource` propio? → El GameObject del botiquín se destruye en el mismo frame; `PlayClipAtPoint` crea un `AudioSource` temporal independiente que sobrevive lo suficiente para terminar el sonido.
- ¿Por qué el tope de curación usa `Mathf.Min` y no un `if`? → Es equivalente pero más compacto; también podría preguntarse cómo se vería con `if (vidaActual > vidaMax) vidaActual = vidaMax;`.

---

## Conceptos transversales que probablemente pregunten

1. **Patrón Singleton simple** (`GameManager.instancia`): por qué se usa (acceso global sin pasar referencias por el Inspector a cada script) y sus riesgos (no hay protección contra múltiples instancias — no hay chequeo `if (instancia != null) Destroy(this)`).
2. **Coroutines vs. `Update()` con timers:** todos los sistemas de espera (recarga, flash de daño, flash de enemigo) usan coroutines con `WaitForSeconds`.
3. **`GetComponentInParent<Vida>()`:** por qué no `GetComponent` directo — el collider que recibe el raycast puede estar en un hijo (ej. un modelo 3D) distinto del GameObject raíz que tiene `Vida`.
4. **Separación de responsabilidades:** `Vida.cs` es genérico y reutilizable (jugador/enemigo); `GameManager.cs` centraliza HUD y estado de partida; cada script de comportamiento (`Disparar`, `EnemigoIA`) solo conoce su propia lógica y notifica al resto vía `GameManager.instancia` o `Vida`.
5. **`Time.timeScale`:** qué afecta (física, animaciones, `Time.deltaTime`) y qué no (Input, UI, `Time.unscaledDeltaTime`).
6. **Tags vs. referencias directas:** `CompareTag("Player")` en `Meta.cs`, `FindGameObjectWithTag("Player")` en `EnemigoIA.cs`.

## Cómo prepararte para el "reto en vivo"

Dado que el reto es modificar tu propio proyecto en vivo, practica cambios pequeños y rápidos sobre cada sistema:
- Cambiar `balasMax`, `tiempoRecarga`, `cadencia` y ver el efecto sin releer código.
- Agregar un segundo tipo de enemigo (duplicar prefab, variar `rangoDisparo`/`dano`/`cadencia`).
- Modificar `RevisarVictoria()` para agregar una tercera condición (ej. tiempo límite).
- Agregar un texto de vida del jugador en el HUD (mismo patrón que `textoMunicion`).
- Ajustar el flash de daño (color, duración) o replicarlo para curación (verde) en el botiquín.

Sabiendo *por qué* está hecho así cada sistema (no solo *qué* hace), deberías poder responder preguntas de "¿qué pasaría si cambias X?" sin necesidad de ejecutar el juego.
