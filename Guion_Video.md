# Guion — Video demo Shooter3DDoom (≈2:45 min)

**Formato:** gameplay grabado + voz en off (o texto en pantalla). Graba con OBS mientras juegas la escena de prueba.

---

## 1. Intro (0:00 – 0:15) — 15 s
**Pantalla:** título del proyecto sobre el menú/escena inicial.

**Voz:** "Este es Shooter3DDoom, un shooter en primera persona estilo retro hecho en Unity. En este video muestro sus mecánicas principales."

---

## 2. Movimiento y cámara (0:15 – 0:35) — 20 s
**Pantalla:** camina con WASD, mira con el mouse, salta un desnivel si hay.

**Voz:** "El jugador se controla con `PrimeraPersona`: WASD para moverse, el mouse para mirar libremente, y un `CharacterController` con gravedad para caminar por el nivel de forma fluida."

**Tip visual:** mostrar el Inspector 2 segundos con los campos `velocidad`, `sensibilidad`, `gravedad`.

---

## 3. Disparo y munición (0:35 – 1:05) — 30 s
**Pantalla:** dispara varias veces contra un enemigo o pared, muestra el HUD de balas bajando, luego recarga con R.

**Voz:** "Con clic izquierdo se dispara un raycast desde el centro de la cámara. Cada disparo consume munición, que se ve reflejada en el HUD. Al quedarse sin balas, se presiona R para iniciar la recarga, con su propio sonido y cadencia de disparo controlada por tiempo."

---

## 4. Enemigos con IA (1:05 – 1:40) — 35 s
**Pantalla:** acércate a un enemigo, muéstralo persiguiendo al jugador con NavMesh, luego cómo se detiene y dispara al estar en rango.

**Voz:** "Los enemigos usan `NavMeshAgent` para perseguir al jugador. Cuando están dentro del rango de disparo, se detienen, lo encaran y disparan periódicamente. Al recibir daño, el enemigo hace un flash rojo como retroalimentación visual, y muere al quedarse sin vida."

**Extra:** si tienen sprites 2D, mencionar `Billboard`: "los sprites de enemigos siempre rotan hacia la cámara, como en los shooters clásicos estilo Doom."

---

## 5. Vida, daño y botiquín (1:40 – 2:05) — 25 s
**Pantalla:** deja que un enemigo te dañe (pantalla roja parpadea), luego camina hacia un botiquín para curarte.

**Voz:** "Cuando el jugador recibe daño, la pantalla parpadea en rojo gracias al `GameManager`. Los botiquines restauran vida al tocarlos, con sonido y desaparición automática."

---

## 6. Condiciones de victoria y derrota (2:05 – 2:35) — 30 s
**Pantalla:** elimina al último enemigo y llega a la meta → panel de victoria. Luego (o con corte) muestra morir → panel de Game Over.

**Voz:** "El `GameManager` controla el estado del juego: al eliminar a todos los enemigos y llegar a la meta, se activa la pantalla de victoria. Si el jugador pierde toda su vida, aparece la pantalla de Game Over con opción de reintentar."

---

## 7. Cierre (2:35 – 2:45) — 10 s
**Pantalla:** logo/título final o vista general del nivel.

**Voz:** "Eso resume las mecánicas de Shooter3DDoom: movimiento, disparo, IA enemiga, vida y condiciones de victoria/derrota, todo hecho en Unity con C#. Gracias por ver."

---

## Checklist de grabación
- [ ] Escena con al menos 2 enemigos visibles y un botiquín cerca del inicio
- [ ] Munición baja para mostrar la recarga
- [ ] Un momento donde te disparen para ver el flash de daño
- [ ] Ruta clara hacia la meta para cerrar con la victoria
- [ ] Grabar Game Over aparte (puedes usar F10, la tecla debug de `GameManager`) y F9 para forzar victoria si el nivel es largo
