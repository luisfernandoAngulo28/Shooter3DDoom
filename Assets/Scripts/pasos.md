# Plan: Parcial Práctico — Shooter3DDoom

## Contexto

Proyecto base "Shooter 3D estilo Doom" en Unity 6, con:
- `Assets/Scripts/PrimeraPersona.cs` — controlador FPS (CharacterController, mouse look, WASD), Input Manager legacy.
- `Assets/Scripts/Disparar.cs` — disparo por raycast desde el centro de cámara, cadencia, sonido, muzzle flash. **Bug existente**: `Invoke("ApagarMuzzle", 0.05f)` llama a un método que no existe; el método real está mal escrito como `ApaagarMuzzle` (línea 48). El muzzle flash nunca se apaga. Se corrige al tocar este archivo.
- `Assets/Scripts/Vida.cs` — vida genérica (jugador/enemigo), `RecibirDano`, jugador recarga la escena al morir, enemigo se destruye.
- Escena `SampleScene`: laberinto de `Pared (n)`, `Piso`/`Piso (1)`, `Techo`, `Jugador` (con `PrimeraPersona`+`Vida`+`CharacterController`), `Arma`+`Muzzle`, `Canvas` vacío, `EventSystem`, `Global Volume`, luz direccional.
- Assets ya disponibles y sin usar aún: sprites `enemigo.png`, `botiquin.png`; sonidos `enemigo_dano.wav`, `jugador_dano.wav`, `victoria.wav`; material `MatEnemigo`.
- Paquetes: `com.unity.ai.navigation` (2.0.12, NavMeshSurface) y `com.unity.ugui` ya instalados — no hace falta importar nada nuevo. No hay NavMesh horneado, ni enemigos, ni UI, ni objeto "Meta" en la escena todavía.

Objetivo: implementar las 5 funciones obligatorias del parcial + botiquín opcional, reusando el estilo de código existente (español, raycast + `Vida.RecibirDano`, `Invoke`/coroutines simples) en vez de introducir un framework nuevo.

## Diseño / Arquitectura

Un `GameManager.cs` nuevo actúa como hub central (singleton simple) para: munición/HUD, contador de enemigos, condición de victoria, panel de Game Over, y el flash de daño. Los scripts existentes lo notifican mediante llamadas directas (`GameManager.instancia.X()`), igual de simple que el patrón `Vida.RecibirDano` que ya usan.

### Scripts nuevos
- `GameManager.cs` — referencias a UI (texto munición, texto enemigos, panel GameOver, panel Victoria, Image de daño). Métodos: `EnemigoMuerto()`, `JugadorEnMeta()`, `RevisarVictoria()`, `GameOver()`, `Reintentar()`, `FlashDano()` (coroutine que sube/baja alpha de una Image roja).
- `EnemigoIA.cs` — en el prefab enemigo. `NavMeshAgent` persigue `transform` del jugador; al entrar en `rangoDisparo` se detiene, mira al jugador y dispara por raycast con cadencia (mismo patrón que `Disparar.cs`), aplicando daño vía `hit.collider.GetComponentInParent<Vida>().RecibirDano(...)`.
- `Meta.cs` — trigger simple: `OnTriggerEnter` detecta al jugador (tag `Player`) y llama `GameManager.instancia.JugadorEnMeta()`.
- `Botiquin.cs` (opcional) — `OnTriggerEnter`, cura al jugador con tope, reproduce sonido con `AudioSource.PlayClipAtPoint` (porque el objeto se destruye) y se destruye.

### Scripts modificados
- `Disparar.cs` — corrige el typo del muzzle; agrega munición (`balasActuales`/`balasMax`), bloqueo de disparo sin balas, recarga con tecla `R` vía coroutine con espera (`tiempoRecarga`), y actualiza el texto de munición en el HUD a través de `GameManager`.
- `Vida.cs` — agrega `Curar(int)` (tope en `vidaMax`, para el botiquín). En `Morir()`: si `esJugador`, en vez de recargar la escena llama a `GameManager.instancia.GameOver()`; si es enemigo, llama a `GameManager.instancia.EnemigoMuerto()` antes de `Destroy`. En `RecibirDano`, si `esJugador`, dispara `GameManager.instancia.FlashDano()`.

## Orden de implementación (por dependencias, no por puntaje)

1. **Fix del bug de muzzle** + preparar `GameManager.cs` vacío con singleton (base para todo lo demás).
2. **Munición y recarga (10 pts)** — extender `Disparar.cs`, HUD de balas.
3. **Enemigos (30 pts)** — `EnemigoIA.cs`, hornear NavMesh (NavMeshSurface sobre el `Piso`), crear prefab enemigo con `NavMeshAgent`+`Vida`+`EnemigoIA`+`AudioSource`, colocar varias instancias en el laberinto.
4. **Contador + victoria doble (5 pts)** — HUD de enemigos restantes en `GameManager`, `Meta.cs` con collider trigger en un punto del laberinto, `RevisarVictoria()` exige `enemigosRestantes == 0 && jugadorEnMeta`.
5. **Game Over + reintentar (5 pts)** — Canvas panel inactivo con texto + botón "Reintentar" (`OnClick` → `GameManager.Reintentar()`), pausa (`Time.timeScale = 0`) y libera cursor al morir.
6. **Feedback de daño (10 pts)** — Image roja full-screen en el Canvas, alpha 0 por defecto; `FlashDano()` la sube y la desvanece con una coroutine.
7. **Botiquín opcional (10 pts)** — `Botiquin.cs` + `Vida.Curar`, usar sprite `botiquin.png` con un trigger collider.

## Pasos de escena (Editor, no vía código)

Como no hay control directo del Editor de Unity desde aquí, estos pasos los harás tú en el Editor guiándote por instrucciones exactas que te daré al entregar cada script (qué componentes agregar, qué campos arrastrar en el Inspector, cómo hornear el NavMesh con `NavMeshSurface`, cómo armar el Canvas con TextMeshPro/Text, botón y panel, y cómo crear el prefab enemigo y sus spawn points). Cada fase de código vendrá acompañada de su checklist de wiring correspondiente.

## Verificación

No hay tests automatizados en un proyecto Unity de este tipo; la verificación es jugando en el Editor (Play mode) después de cada fase:
- Fase 2: disparar hasta vaciar el cargador, confirmar que no dispara sin balas, presionar R y ver el HUD actualizarse tras la espera.
- Fase 3: los enemigos deben perseguir al jugador por el laberinto (respetando paredes vía NavMesh) y dispararle al alcance.
- Fase 4: matar todos los enemigos y pisar la Meta sin haberlos matado a todos primero (no debe ganar); luego sí matarlos y pisar Meta (debe ganar).
- Fase 5: morir y confirmar que aparece el panel con Reintentar, el cursor se libera, y Reintentar reinicia la escena.
- Fase 6: recibir daño y ver el parpadeo rojo.
- Fase 7 (si aplica): tocar el botiquín con vida no máxima y confirmar curación tope + sonido + desaparición.

Al final, el informe de 1 página y el video de 2-3 min quedan a tu cargo (son entregables de documentación/grabación, no de código), pero puedo ayudarte a redactar el informe describiendo las decisiones técnicas una vez el proyecto esté terminado.