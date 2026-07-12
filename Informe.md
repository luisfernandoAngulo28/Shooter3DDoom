# Informe — Shooter 3D estilo Doom

## Sistema de munición y recarga
El arma tiene 12 balas por cargador (`Disparar.cs`). Al llegar a 0 no se puede disparar hasta recargar con **R**, lo que obliga a gestionar el recurso en vez de disparar sin límite. La recarga tiene una espera de 1.5 s (coroutine `Recargar`) para que tenga costo real en combate y no sea instantánea. La munición actual/máxima se muestra siempre en el HUD vía `GameManager.ActualizarUIMunicion`, centralizando toda la UI en un único manager en lugar de que cada script actualice el Canvas por su cuenta.

## Enemigos (IA)
Cada enemigo usa `NavMeshAgent` para perseguir al jugador sobre el NavMesh horneado del nivel, evitando así rutas atravesando paredes. Al entrar en `rangoDisparo` (12 u.) el enemigo se detiene, encara al jugador y dispara por raycast con la misma lógica de daño que usa el jugador (`Vida.RecibirDano`), reutilizando el mismo componente `Vida` tanto para el jugador como para los enemigos (con el flag `esJugador`) en vez de duplicar código de salud en dos clases distintas. La cadencia de disparo enemigo (1.5 s) es más lenta que la del jugador (0.5 s) a propósito, para que el jugador tenga ventaja en un enfrentamiento directo y el desafío venga de que ataquen en grupo.

## Contador de enemigos y condición doble de victoria
`GameManager` lleva la cuenta de enemigos vivos (decrementada en `EnemigoMuerto()`) y un flag separado de si el jugador llegó a la meta (`JugadorEnMeta()`). El nivel solo se da por ganado cuando **ambas** condiciones se cumplen (`RevisarVictoria()`), para evitar que el jugador gane corriendo directo a la meta sin combatir, que era el objetivo del diseño.

## Menú de Game Over con reintento
Al morir el jugador no se recarga la escena automáticamente: se muestra `PanelGameOver` con el botón **Reintentar**, se pausa el juego (`Time.timeScale = 0`) y se libera el cursor para poder interactuar con la UI. El botón llama a `GameManager.Reintentar()`, que recarga la escena activa — la forma más simple y confiable de resetear todo el estado del nivel (enemigos, munición, vida) sin tener que escribir lógica de reseteo manual para cada sistema.

## Feedback de daño
Al recibir daño el jugador ve un flash rojo semitransparente que se desvanece en 0.3 s (`GameManager.FlashDano`, sobre una `Image` de UI a pantalla completa). Se eligió un flash de UI en vez de post-procesado porque es liviano y no depende del render pipeline. Los enemigos, en cambio, tiñen brevemente de blanco su sprite (`Vida.FlashDanoEnemigo`, sobre la propiedad `_BaseColor` del shader URP Lit) para dar feedback de impacto sin necesitar animaciones o sprites adicionales.

## Botiquín (opcional)
Cura vida al tocarlo, con tope en la vida máxima (`Mathf.Min`), reproduce un sonido y se destruye. La vida máxima del jugador se subió a 3 (en vez de 1) específicamente para que este mecanismo tenga sentido: con 1 de vida no hay margen para curarse antes de morir.

## Otras decisiones de pulido
- **Crosshair** fijo en el centro de pantalla, oculto automáticamente al mostrarse cualquier panel (Game Over/Victoria) para no interferir con la UI.
- **Sonidos** asignados por evento (disparo, daño de jugador y enemigo, recarga, victoria, recoger botiquín) para reforzar el feedback de cada acción sin depender solo de lo visual.
- **Teclas de debug** (F9 = forzar victoria, F10 = forzar Game Over) para poder probar ambas pantallas sin jugar el nivel completo — se desactivan con el flag `teclasDebug` en el Inspector antes de la entrega final.
