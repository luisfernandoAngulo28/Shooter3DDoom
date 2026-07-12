# Shooter3DDoom

Shooter en primera persona estilo retro (inspirado en Doom) desarrollado en **Unity** con **C#**, como proyecto de la materia de Programación Gráfica.

## Descripción

El jugador se mueve y dispara en un nivel 3D con enemigos controlados por IA. El objetivo del nivel es doble: eliminar a todos los enemigos **y** llegar a la meta; solo cumpliendo ambas condiciones se gana. Si el jugador pierde toda su vida, se muestra una pantalla de Game Over con opción de reintentar recargando la escena.

## Mecánicas principales

- **Movimiento y cámara** (`PrimeraPersona.cs`): control en primera persona con `CharacterController`, WASD para desplazarse, mouse para mirar y gravedad para caminar por el nivel.
- **Disparo y munición** (`Disparar.cs`): disparo por raycast desde el centro de la cámara, cargador de 12 balas y recarga con **R** (con demora de 1.5 s) para que gestionar munición tenga costo real en combate.
- **Enemigos con IA** (`EnemigoIA.cs`): usan `NavMeshAgent` para perseguir al jugador por el NavMesh del nivel; al entrar en rango de disparo se detienen, encaran al jugador y disparan con la misma lógica de daño que él, con cadencia más lenta para darle ventaja en enfrentamientos 1 a 1.
- **Vida y daño** (`Vida.cs`): componente compartido entre jugador y enemigos (flag `esJugador`) para no duplicar lógica de salud; incluye feedback visual (flash rojo en el jugador, flash blanco en enemigos).
- **Botiquín** (`Botiquin.cs`): cura al jugador hasta su vida máxima al tocarlo, con sonido y autodestrucción.
- **Meta y condición de victoria** (`Meta.cs`, `GameManager.cs`): el `GameManager` centraliza el estado del juego (HUD de munición/vida, conteo de enemigos vivos, condición de victoria, panel de Game Over y reintento).
- **Billboard** (`Billboard.cs`): rota los sprites de los enemigos hacia la cámara, como en los shooters clásicos 2.5D.

## Documentación adicional

- [`Informe.md`](Informe.md) — justificación de las decisiones de diseño e implementación.
- [`Guion_Video.md`](Guion_Video.md) — guion para el video demo del proyecto.
