
---

# **DOCUMENTACIÓN GDD**

## **Título del Juego**
**Project Arkanoid**

---

## **1. Visión General del Juego**

### **Concepto Básico**
"Project Arkanoid" es un juego arcade clásico inspirado en el icónico "Arkanoid". El jugador controla una raqueta que debe golpear una bola para destruir bloques en la pantalla. A medida que avanzan los niveles, se introducen nuevos desafíos, power-ups y obstáculos que aumentan la dificultad y diversión.

### **Género**
- **Género Principal**: Arcade
- **Subgénero**: Rompebloques (Breakout)

### **Plataforma Objetivo**
- **Plataformas**: PC (Windows, macOS)
- **Dispositivos**: Teclado y ratón (PC)

---

## **2. Mecánicas de Juego**

### **Movimiento del Paddle**
- La raqueta se mueve horizontalmente en la parte inferior de la pantalla.
- Controlada por el jugador mediante las flechas izquierda/derecha.
- Velocidad ajustable según power-ups.

### **Comportamiento de la Bola**
- La bola rebota en las paredes, el paddle y los bloques.
- Rebota en ángulos según el punto de impacto en el paddle.
- Reinicio si la bola cae fuera de la pantalla.
- La velocidad de la bola aumenta gradualmente con el tiempo.

#### **Detalles Técnicos**:
- La bola utiliza un sistema de pooling (`ObjectPoolManager`) para optimizar su creación y reutilización.
- La velocidad inicial de la bola está configurada en `100.0f` y aumenta un `10%` cada `5 segundos`.

### **Tipos de Bloques y sus Características**
- **Normales**: Se destruyen con un golpe
    - **Bloques Azules**: +3 puntos
    - **Bloques Verdes**: +4 puntos
    - **Bloques Rojos**: -1 punto
    - **Bloques Rosas**: +5 puntos y liberan power-ups al ser destruidos.
- **Explosivos**: Causan una explosión que afecta a bloques cercanos.
- **Indestructibles**: Obstáculos fijos en el nivel.

#### **Detalles Técnicos**:
- Los bloques utilizan un sistema de pooling similar al de la bola.
- Los bloques tienen diferentes comportamientos según su tipo (`BlockColor`).

### **Sistema de Power-Ups**
- **Big Racket**: Aumenta temporalmente el tamaño de la raqueta.
- **Multi Ball**: Genera bolas adicionales.
- **Slow Ball**: Reduce la velocidad de la bola durante 10 segundos.
- **Extra Life**: Otorga una vida adicional.
- **Fast Racket**: Aumenta temporalmente la velocidad de la raqueta.
- **Slow Racket**: Reduce temporalmente la velocidad de la raqueta.

#### **Detalles Técnicos**:
- Los power-ups están representados por el enum `BonusType`.
- Se generan aleatoriamente cuando se destruyen bloques especiales.

### **Condiciones de Victoria y Derrota**
- **Victoria**: Destruir todos los bloques azules en un nivel.
- **Derrota**: Perder todas las vidas antes de completar el nivel.

---

## **3. Programación**

### **Estructura de Clases**
- **GameManager**: Gestiona el estado global del juego (puntuación, vidas, niveles).
- **Ball**: Controla el comportamiento de la bola (movimiento, colisiones).
- **Block**: Define los tipos de bloques y sus interacciones.
- **Racket**: Maneja el movimiento y los efectos de la raqueta.
- **Bonus**: Implementa los power-ups y sus efectos.
- **UIManager**: Controla la interfaz gráfica (HUD, menús).
- **MusicManager**: Gestiona la música y los efectos de sonido.
- **ObjectPoolManager**: Implementa un sistema de pooling para objetos como bolas y power-ups.

### **Estructura de Control**
- **Singletons**: GameManager, UIManager y MusicManager son singletons para persistir entre escenas.
- **Pooling**: Uso de un sistema de pooling para objetos como bolas y power-ups.
- **Corrutinas**: Para manejar eventos temporales como power-ups y explosiones.

---

## **4. Diseño de Niveles**

### **Estructura de los Niveles**
- Cada nivel tiene una disposición única de bloques.

### **Progresión de Dificultad**
- La disposición de bloques y obstaculos aumenta gradualmente su dificultad por nivel.
- Aumento de la velocidad de la bola.

### **Variaciones en la Disposición de los Bloques**
- Patrones simétricos, asimétricos y laberínticos.
- Obstáculos indestructibles que bloquean el camino de la bola.

---

## **5. Sistema de Puntuación**

### **Cómo se Ganán Puntos**
- Destruir bloques: otorga puntos según su tipo.
    - Azules: +3 puntos
    - Verdes: +4 puntos
    - Rojos: -1 punto
    - Rosas: +5 puntos
- Recoger power-ups: +30 puntos por cada power-up recogido.

### **Puntuaciones Altas**
- Se guarda la puntuación máxima en PlayerPrefs para mostrarla en el menú principal.

---

## **6. Interfaz de Usuario**

### **HUD (Heads-Up Display)**
- **Puntuación Máxima**: Mostrada en la esquina superior derecha.
- **Puntuación Actual**: Mostrada en la esquina superior derecha.
- **Vidas Restantes**: Representadas por iconos de corazones en la esquina superior izquierda.
- **Nivel Actual**: Mostrado en la esquina inferior derecha.

### **Menús del Juego**
- **Menú Principal**: Opciones para jugar, salir y ver la puntuación máxima.
- **Pantalla de Game Over**: Muestra la puntuación final y un botón para reiniciar.

---

## **7. Arte y Estilo Visual**

### **Estilo Gráfico**
- **Estilo**: Pixel art retro con colores vibrantes.
- **Inspiración**: Juegos clásicos de arcade de los años 80 y 90

### **Paleta de Colores**
- Colores vibrantes para los bloques y power-ups.
- Fondo oscuro para destacar los elementos del juego.

### **Diseño de Sprites**
- Raqueta: Rectángulo simple con detalles metálicos.
- Bola: Esfera brillante con efectos de luz.
- Bloques: Variaciones de color según el tipo de bloque.
- Power-ups: Iconos pequeños y coloridos.
- Explosiones de partículas.

---

## **8. Audio**

### **Efectos de Sonido**
- Start Game: Sonido al presionar el boton de inicio.
- Golpe de bola: Sonido al golpear el paddle o los bloques.
- Bloques indestructibles: Sonido al golpear una roca.
- Power-Up: Sonido al recoger un power-up.
- Explosión: Sonido al destruir un bloque explosivo.
- Vidas: Sonido al ganar o perder una vida.
- Game Over: Sonido al perder todas las vidas.

### **Música de Fondo**
- Temas principal de Arkanoid en menú principal.

---

## **9. Características Técnicas**

### **Motor de Juego**
- Unity con lenguaje C#.

### **Requisitos de Rendimiento**
- Compatible con dispositivos de gama media/baja.
- Optimización para pantallas windowed.

---

## **10. Planificación del Desarrollo: Fases del Desarrollo**

### **Fase 1: Preproducción**
- Diseño de mecánicas básicas.
- Creación de prototipos.
- Definición de arte y estilo visual.

### **Fase 2: Desarrollo inicial**
- Implementación de mecánicas básicas (paddle, bola, bloques).
- Creación de niveles iniciales.
- Implementación de la interfaz de usuario básica.

### **Fase 3: Desarrollo avanzado**
- Implementación de power-ups.
- Creación de niveles adicionales.
- Integración de sonidos y música.

### **Fase 4: Polishing**
- Mejoras visuales y de audio.
- Optimización del rendimiento.

### **Fase 5: Pruebas y ajustes**
- Pruebas de jugabilidad.
- Corrección de errores y optimización.

### **Fase 6: Lanzamiento**
- Publicación en plataformas objetivo.

---
