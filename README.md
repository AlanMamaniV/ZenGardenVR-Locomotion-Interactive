# Zen Garden VR: Japanese Kyoto Experience (Interactive Edition)

Experiencia de Realidad Virtual inmersiva ambientada en un auténtico **Jardín Zen tradicional japonés de Kioto**, desarrollada en **Unity 6 (6000.6.8f1)** utilizando **OpenXR**, **Universal Render Pipeline (URP 17.6.0)** y **XR Interaction Toolkit 3.6.1**.

Esta edición expande significativamente el diseño visual, espacial y las mecánicas interactivas del proyecto, combinando elementos arquitectónicos clásicos (Torii, Faroles Tōrō, Shishi-odoshi, Pagoda de Contemplación) con 4 interacciones especiales de manipulación física, retroalimentación sonora y efectos atmosféricos.

> 🇬🇧 **English Summary:** An immersive Japanese Zen Garden VR experience built with Unity 6 & URP featuring authentic Kyoto aesthetics, anti-nausea parabolic teleportation, World Space UI, object selection with highlight, physics-based rock arrangement, interactive lanterns, bonsai contemplations, and traditional Tsukubai bamboo water fountains.

---

## 1. Zonas del Jardín Zen de Kioto

El entorno 3D ha sido rediseñado bajo una paleta cromática orgánica y armónica (verde musgo, arena rastrillada beige, madera oscura envejecida, piedra gris de río y toques de rojo bermellón tradicional):

1. **Zona 1: Entrada Tradicional con Torii:**
   * Majestuoso portal Torii rojo bermellón con vigas Kasagi y Nuki.
   * Flanqueado por faroles de piedra Tōrō de bienvenida y sendero central de losas de piedra.
2. **Zona 2: Jardín de Arena Karesansui (Jardín Seco):**
   * Delimitado con marco de madera oscura tradicional.
   * Cama de arena con surcos concéntricos rastrillados.
   * Disposición de rocas maestras *Sanzon Iwagumi* (tres piedras en equilibrio cósmico).
   * **Interacción Especial 1:** Piedra sagrada manipulable con agarre físico (`XRGrabInteractable`) y acople armónico al círculo sagrado de contemplación.
3. **Zona 3: Pabellón de Bonsái Centenario:**
   * Plataforma de madera con mesa de exposición y maceta de cerámica.
   * **Interacción Especial 2:** Bonsái de flor de cerezo (Sakura) con tronco retorcido y lluvia de pétalos rosados al interactuar.
4. **Zona 4: Avenida de Faroles Tōrō:**
   * Cuatro faroles tradicionales tallados en piedra de Kioto distribuidos por el jardín.
   * **Interacción Especial 3:** Farol Tōrō interactivo que permite encender y apagar la llama cálida interior (`Light` de punto cálido y núcleo emisivo).
5. **Zona 5: Estanque de Piedras y Fuente Tsukubai (Shishi-odoshi):**
   * Estanque con agua translúcida y reflexiones en tiempo real.
   * Borde perimetral delimitado con rocas naturales.
   * **Interacción Especial 4:** Pileta ritual de piedra con caña de bambú basculante que activa y detiene el flujo de agua y su audio relajante.
6. **Zona 6: Pabellón de Contemplación y Té:**
   * Plataforma elevada con escalones transitables por teletransporte.
   * Columnas de madera, tejado pagoda tradicional, bancos de contemplación y mesa ceremonial con cuenco de té.
7. **Vegetación Perimetral:**
   * Bosque de cañas de bambú natural y árboles de Sakura en floración completa.

---

## 2. Sistema de Interacciones VR (4 Mecánicas Implementadas)

Todas las interacciones cuentan con **resaltado visual al apuntar (`ObjectHighlight.cs`)**, **inspección con ficha descriptiva en la UI (`ObjectSelectionManager.cs`)** y respuesta háptica/sonora:

| Elemento | Tipo de Interactor | Mecánica e Interacción Especial |
|---|---|---|
| **Piedra Zen Sagrada** | Direct & Ray Grab (`XRGrabInteractable`) | Agarre físico con gravedad. Al soltarla dentro del círculo sagrado en la arena, se acopla, emite un destello dorado y actualiza el contador. |
| **Bonsái de Sakura** | Ray Interactor (`XRSimpleInteractable`) | Al seleccionarse, reproduce sonido de campanilla japonesa, despliega su significado zen y detona una suave cascada de pétalos de cerezo. |
| **Farol Tōrō** | Ray Interactor (`XRSimpleInteractable`) | Activa y apaga la luz puntual cálida (2.0 lux, tono ámbar) y el fuego interior con sonido de encendido. |
| **Fuente Tsukubai** | Ray Interactor (`XRSimpleInteractable`) | Abre o cierra el chorro de agua de la caña de bambú y muta el audio relajante del flujo constante. |

---

## 3. Navegación VR y Confort Anti-Náuseas

- **Locomoción Principal:** Teletransporte parabólico instantáneo configurado en 19 zonas y losas (`TeleportationArea`).
- **Giro Suave vs. Discreto:** Snap Turn angular configurado a 45° para eliminar la cinetosis o sensación de mareo.
- **Transición y Orientación:** Conservación de la orientación vertical del jugador (`MatchOrientation.WorldSpaceUp`).
- **Simulador Integrado:** Compatible al 100% con **XR Interaction Simulator** para evaluar directamente en el Editor de Unity usando teclado y ratón.

---

## 4. Interfaz Gráfica en VR (World Space UI)

Tablón de madera natural integrado ergonómicamente en la entrada del jardín:
- **Título Zen:** "JARDÍN ZEN DE KIOTO".
- **Objetivo:** Instrucciones de contemplación y serenidad.
- **Ficha Dinámica del Objeto:** Muestra el nombre, descripción cultural y controles del elemento al que se apunta con el rayo.
- **Contador de Armonía:** Registra el avance de elementos descubiertos `(0 / 4)`.
- **Feed de Notificaciones:** Mensajes efímeros que confirman cada interacción realizada.

---

## 5. Especificaciones Técnicas

- **Motor:** Unity 6000.6.8f1 (Unity 6).
- **Render Pipeline:** Universal Render Pipeline (URP) 17.6.0.
- **XR Framework:** XR Interaction Toolkit 3.6.1 & XR Core Utils 2.6.0.
- **XR Provider:** OpenXR Plugin 1.18.0.
- **Input System:** Unity Input System 1.20.1.
- **Audio & UI:** UGUI 2.6.0 con TextMesh Pro.

---

## 6. Estructura del Proyecto

```
Assets/
  Scenes/
    MainVRScene.unity                    <-- ESCENA PRINCIPAL JARDÍN ZEN
    MainVRScene_MagicalIsland_Backup.unity
  Scripts/
    Interactions/
      ObjectHighlight.cs                 <-- Resaltado visual emisivo al apuntar
      ObjectSelectionManager.cs          <-- Envío de nombres y fichas descriptivas a la UI
      ZenStoneInteraction.cs             <-- Agarre físico y acople en círculo de arena
      BonsaiInteraction.cs               <-- Activación de pétalos y balanceo con brisa
      LanternInteraction.cs              <-- Encendido y apagado de faroles Tōrō
      FountainInteraction.cs             <-- Control de chorro y audio de la fuente Tsukubai
      VRInteractionUI.cs                 <-- Panel World Space informativo
    Environment/
      AmbientAudioController.cs          <-- Audio ambiental relajante (agua, brisa)
      EnvironmentEffects.cs              <-- Movimiento de vegetación y partículas
      WaterController.cs                 <-- Ondulaciones del estanque de agua
  Materials/ZenGarden/
    M_ZenSand.mat                        <-- Arena rastrillada Karesansui
    M_ZenMoss.mat                        <-- Césped verde musgo tradicional
    M_ToriiRed.mat                       <-- Bermellón tradicional japonés
    M_ZenWoodDark.mat                    <-- Madera oscura envejecida
    M_ZenBamboo.mat                      <-- Bambú natural
    M_ZenStoneGray.mat                   <-- Piedra gris de Kioto
    M_SakuraPink.mat                     <-- Pétalos de Sakura
    M_ZenWaterPond.mat                   <-- Agua translúcida
    M_ZenLanternFlame.mat                <-- Núcleo de llama cálida
    M_ZenWhitePebble.mat                 <-- Guijarros blancos de río
  Editor/
    ZenGarden/ZenGardenBuilder.cs        <-- Menú Tools > Zen Garden VR > Transform Scene
```

---

## 7. Instrucciones de Ejecución

1. Abrir el proyecto en **Unity 6000.6.8f1**.
2. En la ventana **Project**, abrir:
   `Assets/Scenes/MainVRScene.unity`.
3. Presionar **Play (▶)**:
   * **Con Visor VR (Meta Quest 2/3/Pro, Vive, Index):** Apuntar con el joystick o gatillo al sendero/arena para teletransportarse; usar Grip para agarrar la piedra Zen y Trigger para interactuar con los faroles, bonsái y fuente.
   * **Sin Visor (XR Device Simulator):** Usar ratón y teclado mediante el simulador integrado en el proyecto.
