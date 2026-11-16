using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using TMPro;
using ZenGarden.Interactions;
using ZenGarden.Environment;
using VRExplorer;

namespace ZenGarden.Editor
{
    public static class ZenGardenBuilder
    {
        private const string ScenePath = "Assets/Scenes/MainVRScene.unity";

        [MenuItem("Tools/Zen Garden VR/Transform Scene to Japanese Zen Garden", false, 1)]
        public static void TransformToZenGardenMenu()
        {
            BuildZenGardenScene();
        }

        public static void BuildZenGardenScene()
        {
            Debug.Log("[Zen Garden VR] Iniciando rediseño visual y espacial a Jardín Zen Japonés...");

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Cargar Materiales Zen
            ZenMaterials mats = LoadZenMaterials();

            // 2. Configurar Atmósfera de Atardecer de Kioto e Iluminación Cálida
            SetupZenAtmosphere();

            // 3. Configurar XR Rig & Teleportation Provider
            GameObject xrOriginGO = SetupVRLocomotionRig();

            // 4. Construir Entorno 3D Arquitectónico y Natural del Jardín Zen
            GameObject gardenRoot = new GameObject("--- Japanese Zen Garden Environment ---");
            BuildZenTerrain(gardenRoot.transform, mats);
            BuildToriiEntrance(gardenRoot.transform, mats);
            BuildRakedSandGarden(gardenRoot.transform, mats);
            BuildBonsaiPavilion(gardenRoot.transform, mats);
            BuildLanternAvenue(gardenRoot.transform, mats);
            BuildTsukubaiWaterGarden(gardenRoot.transform, mats);
            BuildContemplationTeaPavilion(gardenRoot.transform, mats);
            BuildBambooAndFlora(gardenRoot.transform, mats);

            // 5. Interfaz Gráfica World Space Zen
            SetupZenWorldSpaceUI(gardenRoot.transform, mats);

            // 6. Configurar Sistema de Audio y Atmósfera
            SetupZenAtmosphereControllers(gardenRoot.transform);

            // Guardar Escena
            EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log($"[Zen Garden VR] ¡Jardín Zen Japonés generado y guardado exitosamente en {ScenePath}!");
        }

        #region Materiales
        private class ZenMaterials
        {
            public Material sand;
            public Material moss;
            public Material toriiRed;
            public Material darkWood;
            public Material bamboo;
            public Material stoneGray;
            public Material sakuraPink;
            public Material waterPond;
            public Material lanternFlame;
            public Material whitePebble;
        }

        private static ZenMaterials LoadZenMaterials()
        {
            ZenMaterials m = new ZenMaterials();
            m.sand = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ZenGarden/M_ZenSand.mat");
            m.moss = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ZenGarden/M_ZenMoss.mat");
            m.toriiRed = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ZenGarden/M_ToriiRed.mat");
            m.darkWood = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ZenGarden/M_ZenWoodDark.mat");
            m.bamboo = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ZenGarden/M_ZenBamboo.mat");
            m.stoneGray = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ZenGarden/M_ZenStoneGray.mat");
            m.sakuraPink = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ZenGarden/M_SakuraPink.mat");
            m.waterPond = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ZenGarden/M_ZenWaterPond.mat");
            m.lanternFlame = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ZenGarden/M_ZenLanternFlame.mat");
            m.whitePebble = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ZenGarden/M_ZenWhitePebble.mat");
            return m;
        }
        #endregion

        #region Atmósfera e Iluminación
        private static void SetupZenAtmosphere()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.95f, 0.72f, 0.58f);    // Naranja suave atardecer
            RenderSettings.ambientEquatorColor = new Color(0.70f, 0.62f, 0.55f);// Tonos tierra cálidos
            RenderSettings.ambientGroundColor = new Color(0.28f, 0.35f, 0.22f); // Verde musgo
            RenderSettings.ambientIntensity = 1.05f;

            // Niebla suave atmosférica japonesa
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.007f;
            RenderSettings.fogColor = new Color(0.92f, 0.82f, 0.75f);

            // Sol natural del atardecer
            GameObject sunGO = new GameObject("Directional Light - Sunset Sun");
            sunGO.transform.rotation = Quaternion.Euler(30f, -40f, 0f);
            Light sunLight = sunGO.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.color = new Color(1.0f, 0.88f, 0.75f);
            sunLight.intensity = 1.25f;
            sunLight.shadows = LightShadows.Soft;

            var lightData = sunGO.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalLightData>();
            lightData.usePipelineSettings = true;
        }
        #endregion

        #region Locomoción VR
        private static GameObject SetupVRLocomotionRig()
        {
            GameObject xriManager = new GameObject("XR Interaction Manager");
            xriManager.AddComponent<XRInteractionManager>();

            GameObject xrOriginGO = new GameObject("XR Origin (XR Rig)");
            xrOriginGO.transform.position = new Vector3(0f, 0f, -12f); // Posición inicial en la entrada del Torii
            var xrOrigin = xrOriginGO.AddComponent<XROrigin>();

            GameObject cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.SetParent(xrOriginGO.transform, false);
            xrOrigin.CameraFloorOffsetObject = cameraOffset;

            GameObject mainCamera = new GameObject("Main Camera");
            mainCamera.transform.SetParent(cameraOffset.transform, false);
            mainCamera.tag = "MainCamera";
            var cam = mainCamera.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 250f;
            mainCamera.AddComponent<AudioListener>();
            xrOrigin.Camera = cam;

            // Controllers Izquierdo y Derecho con Ray Interactor
            SetupController(cameraOffset.transform, "Left Controller", true);
            SetupController(cameraOffset.transform, "Right Controller", false);

            // Locomotion Mediators (Teletransporte + Snap Turn 45°)
            var locomotionMediator = xrOriginGO.AddComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionMediator>();
            var teleportProvider = xrOriginGO.AddComponent<TeleportationProvider>();
            var snapTurn = xrOriginGO.AddComponent<SnapTurnProvider>();
            snapTurn.turnAmount = 45f;

            // Manager de setup de confort
            xrOriginGO.AddComponent<VRSetupManager>();

            return xrOriginGO;
        }

        private static void SetupController(Transform parent, string name, bool isLeft)
        {
            GameObject ctrl = new GameObject(name);
            ctrl.transform.SetParent(parent, false);
            ctrl.transform.localPosition = new Vector3(isLeft ? -0.2f : 0.2f, -0.1f, 0.3f);

            var interactor = ctrl.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
            ctrl.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>();
        }
        #endregion

        #region Construcción del Entorno Japonés
        private static void BuildZenTerrain(Transform root, ZenMaterials mats)
        {
            GameObject terrain = new GameObject("Zen_MainGround");
            terrain.transform.SetParent(root, false);

            // Suelo base verde musgo
            GameObject baseMoss = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseMoss.name = "Ground_MossCircle";
            baseMoss.transform.SetParent(terrain.transform, false);
            baseMoss.transform.position = new Vector3(0f, -0.05f, 5f);
            baseMoss.transform.localScale = new Vector3(45f, 0.1f, 45f);
            baseMoss.GetComponent<MeshRenderer>().sharedMaterial = mats.moss;

            // Área de teletransporte principal sobre el césped
            var tpArea = baseMoss.AddComponent<TeleportationArea>();
            tpArea.matchOrientation = MatchOrientation.WorldSpaceUp;

            // Camino ceremonial central de piedras de río planas
            GameObject pathGroup = new GameObject("Stone_Pathway");
            pathGroup.transform.SetParent(terrain.transform, false);

            for (int i = -14; i <= 20; i += 2)
            {
                GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                stone.name = $"PathStone_{i}";
                stone.transform.SetParent(pathGroup.transform, false);
                stone.transform.position = new Vector3((i % 4 == 0 ? 0.15f : -0.15f), 0.02f, i);
                stone.transform.localScale = new Vector3(1.2f, 0.05f, 1.0f);
                stone.GetComponent<MeshRenderer>().sharedMaterial = mats.stoneGray;

                var pTp = stone.AddComponent<TeleportationArea>();
                pTp.matchOrientation = MatchOrientation.WorldSpaceUp;
            }
        }

        private static void BuildToriiEntrance(Transform root, ZenMaterials mats)
        {
            GameObject entrance = new GameObject("Zone1_ToriiEntrance");
            entrance.transform.SetParent(root, false);
            entrance.transform.position = new Vector3(0f, 0f, -11f);

            // Pilares Torii Rojos
            CreateCylinder(entrance.transform, "Torii_Pillar_L", new Vector3(-1.8f, 2.2f, 0f), new Vector3(0.35f, 2.2f, 0.35f), mats.toriiRed);
            CreateCylinder(entrance.transform, "Torii_Pillar_R", new Vector3(1.8f, 2.2f, 0f), new Vector3(0.35f, 2.2f, 0.35f), mats.toriiRed);

            // Viga horizontal superior curva tradicional (Kasagi)
            CreateCube(entrance.transform, "Torii_Kasagi_Top", new Vector3(0f, 4.3f, 0f), new Vector3(5.2f, 0.35f, 0.45f), mats.toriiRed);
            CreateCube(entrance.transform, "Torii_Kasagi_Cap", new Vector3(0f, 4.5f, 0f), new Vector3(5.5f, 0.1f, 0.55f), mats.darkWood);

            // Viga horizontal secundaria (Nuki)
            CreateCube(entrance.transform, "Torii_Nuki", new Vector3(0f, 3.5f, 0f), new Vector3(4.2f, 0.25f, 0.3f), mats.toriiRed);

            // Faroles de bienvenida a los costados
            CreateJapaneseLantern(entrance.transform, new Vector3(-2.8f, 0f, 0f), mats, true);
            CreateJapaneseLantern(entrance.transform, new Vector3(2.8f, 0f, 0f), mats, false);
        }

        private static void BuildRakedSandGarden(Transform root, ZenMaterials mats)
        {
            GameObject sandZone = new GameObject("Zone2_Karesansui_SandGarden");
            sandZone.transform.SetParent(root, false);
            sandZone.transform.position = new Vector3(-8f, 0.03f, 0f);

            // Marco de madera oscura del jardín de arena
            CreateCube(sandZone.transform, "SandFrame_Border", new Vector3(0f, 0.1f, 0f), new Vector3(10f, 0.2f, 10f), mats.darkWood);

            // Cama de arena rastrillada (Karesansui)
            GameObject sandPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sandPlane.name = "RakedSand_Surface";
            sandPlane.transform.SetParent(sandZone.transform, false);
            sandPlane.transform.position = new Vector3(0f, 0.12f, 0f);
            sandPlane.transform.localScale = new Vector3(9.4f, 0.08f, 9.4f);
            sandPlane.GetComponent<MeshRenderer>().sharedMaterial = mats.sand;

            var sandTp = sandPlane.AddComponent<TeleportationArea>();
            sandTp.matchOrientation = MatchOrientation.WorldSpaceUp;

            // Ondulaciones / Surcos de arena Zen
            for (int r = 0; r < 4; r++)
            {
                CreateCylinder(sandZone.transform, $"SandRakeRipples_{r}", new Vector3(0f, 0.16f, 0f), new Vector3(1.5f + r * 1.8f, 0.02f, 1.5f + r * 1.8f), mats.sand);
            }

            // Rocas Zen Sagradas (Disposición tradicional Sanzon Iwagumi: 3 rocas en equilibrio)
            GameObject rockCenter = CreateSphere(sandZone.transform, "ZenStone_Master", new Vector3(0f, 0.7f, 0f), new Vector3(1.3f, 1.8f, 1.2f), mats.stoneGray);
            GameObject rockL = CreateSphere(sandZone.transform, "ZenStone_Secondary", new Vector3(-1.4f, 0.45f, 0.5f), new Vector3(0.9f, 0.9f, 0.8f), mats.stoneGray);
            GameObject rockR = CreateSphere(sandZone.transform, "ZenStone_Tertiary", new Vector3(1.2f, 0.4f, -0.6f), new Vector3(0.7f, 0.7f, 0.8f), mats.stoneGray);

            // Círculo sagrado de colocación
            GameObject targetCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            targetCircle.name = "ZenCircle_PlacementTarget";
            targetCircle.transform.SetParent(sandZone.transform, false);
            targetCircle.transform.position = new Vector3(0f, 0.17f, 2.5f);
            targetCircle.transform.localScale = new Vector3(1.4f, 0.03f, 1.4f);
            targetCircle.GetComponent<MeshRenderer>().sharedMaterial = mats.whitePebble;

            // Partículas de iluminación armónica para la colocación
            ParticleSystem pSys = CreatePlacementParticles(targetCircle.transform);

            // INTERACCIÓN ESPECIAL 1: Piedra Zen manipulable con física y snap armónico
            GameObject movableStone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            movableStone.name = "Interactive_ZenStone";
            movableStone.transform.SetParent(sandZone.transform, false);
            movableStone.transform.position = new Vector3(-3.2f, 0.6f, -2.5f);
            movableStone.transform.localScale = new Vector3(0.5f, 0.45f, 0.55f);
            movableStone.GetComponent<MeshRenderer>().sharedMaterial = mats.stoneGray;

            var rb = movableStone.AddComponent<Rigidbody>();
            rb.mass = 2.5f;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            var grab = movableStone.AddComponent<XRGrabInteractable>();
            movableStone.AddComponent<ObjectHighlight>();

            var selMgr = movableStone.AddComponent<ObjectSelectionManager>();
            SetSelectionManagerData(selMgr, "Piedra Sagrada Zen", "Roca pulida de río utilizada para meditación Karesansui.", "• Agarra con Grip y colócala en el círculo sagrado de arena.");

            var zenInt = movableStone.AddComponent<ZenStoneInteraction>();
            SerializedObject soZ = new SerializedObject(zenInt);
            soZ.FindProperty("targetZenCircle").objectReferenceValue = targetCircle.transform;
            soZ.FindProperty("placementParticles").objectReferenceValue = pSys;
            soZ.FindProperty("placementClip").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Button Pop.wav");
            soZ.ApplyModifiedProperties();
        }

        private static void BuildBonsaiPavilion(Transform root, ZenMaterials mats)
        {
            GameObject bonsaiZone = new GameObject("Zone3_BonsaiGarden");
            bonsaiZone.transform.SetParent(root, false);
            bonsaiZone.transform.position = new Vector3(8f, 0f, -2f);

            // Plataforma de madera elevada para bonsáis
            CreateCube(bonsaiZone.transform, "Bonsai_Deck", new Vector3(0f, 0.25f, 0f), new Vector3(5f, 0.5f, 5f), mats.darkWood);
            CreateCube(bonsaiZone.transform, "Bonsai_StandTable", new Vector3(0f, 0.75f, 0f), new Vector3(1.6f, 0.5f, 1.2f), mats.darkWood);

            // Maceta de bonsái de cerámica tradicional
            CreateCube(bonsaiZone.transform, "Bonsai_Pot", new Vector3(0f, 1.1f, 0f), new Vector3(1.1f, 0.25f, 0.8f), mats.stoneGray);

            // INTERACCIÓN ESPECIAL 2: Bonsái Centenario de Cerezo
            GameObject bonsaiRoot = new GameObject("Interactive_Bonsai_Tree");
            bonsaiRoot.transform.SetParent(bonsaiZone.transform, false);
            bonsaiRoot.transform.position = new Vector3(0f, 1.25f, 0f);

            // Tronco retorcido artístico
            CreateCylinder(bonsaiRoot.transform, "Trunk_Base", new Vector3(0f, 0.35f, 0f), new Vector3(0.2f, 0.35f, 0.18f), mats.darkWood);
            CreateCylinder(bonsaiRoot.transform, "Trunk_Branch1", new Vector3(0.25f, 0.7f, 0.1f), new Vector3(0.12f, 0.3f, 0.12f), mats.darkWood, Quaternion.Euler(0, 0, -35f));
            CreateCylinder(bonsaiRoot.transform, "Trunk_Branch2", new Vector3(-0.25f, 0.8f, -0.1f), new Vector3(0.1f, 0.25f, 0.1f), mats.darkWood, Quaternion.Euler(0, 0, 40f));

            // Follaje de Sakura rosa floreciente
            CreateSphere(bonsaiRoot.transform, "Sakura_Canopy_1", new Vector3(0.45f, 1.0f, 0.15f), new Vector3(0.75f, 0.55f, 0.7f), mats.sakuraPink);
            CreateSphere(bonsaiRoot.transform, "Sakura_Canopy_2", new Vector3(-0.4f, 1.1f, -0.1f), new Vector3(0.65f, 0.5f, 0.65f), mats.sakuraPink);
            CreateSphere(bonsaiRoot.transform, "Sakura_Canopy_Top", new Vector3(0.05f, 1.25f, 0f), new Vector3(0.85f, 0.6f, 0.8f), mats.sakuraPink);

            // Partículas de pétalos de flor de cerezo
            ParticleSystem sakuraPS = CreateSakuraPetalParticles(bonsaiRoot.transform);

            // Interactable y componentes
            var boxCol = bonsaiRoot.AddComponent<BoxCollider>();
            boxCol.size = new Vector3(1.4f, 1.6f, 1.4f);
            boxCol.center = new Vector3(0f, 0.8f, 0f);

            var sInt = bonsaiRoot.AddComponent<XRSimpleInteractable>();
            bonsaiRoot.AddComponent<ObjectHighlight>();

            var selMgr = bonsaiRoot.AddComponent<ObjectSelectionManager>();
            SetSelectionManagerData(selMgr, "Bonsái de Sakura Centenario", "Símbolo de la belleza efímera y la renovación espiritual en la cultura japonesa.", "• Apunta y presiona Trigger para contemplar su lluvia de pétalos.");

            var bonsaiInt = bonsaiRoot.AddComponent<BonsaiInteraction>();
            SerializedObject soB = new SerializedObject(bonsaiInt);
            soB.FindProperty("cherryBlossomParticles").objectReferenceValue = sakuraPS;
            soB.FindProperty("treeModel").objectReferenceValue = bonsaiRoot.transform;
            soB.FindProperty("chimeClip").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Button Pop.wav");
            soB.ApplyModifiedProperties();
        }

        private static void BuildLanternAvenue(Transform root, ZenMaterials mats)
        {
            GameObject lanternGroup = new GameObject("Zone4_StoneLantern_Avenue");
            lanternGroup.transform.SetParent(root, false);

            // Faroles Tōrō distribuidos por los senderos
            Vector3[] lanternPositions = new Vector3[]
            {
                new Vector3(-2.2f, 0f, -4f),
                new Vector3(2.2f, 0f, 3f),
                new Vector3(-2.2f, 0f, 10f),
                new Vector3(2.2f, 0f, 15f)
            };

            for (int i = 0; i < lanternPositions.Length; i++)
            {
                bool isInteractive = (i == 1); // El farol de la derecha es la INTERACCIÓN ESPECIAL 3
                CreateJapaneseLantern(lanternGroup.transform, lanternPositions[i], mats, isInteractive);
            }
        }

        private static void CreateJapaneseLantern(Transform parent, Vector3 pos, ZenMaterials mats, bool isInteractive)
        {
            GameObject lantern = new GameObject(isInteractive ? "Interactive_ToroLantern" : "StoneLantern_Toro");
            lantern.transform.SetParent(parent, false);
            lantern.transform.position = pos;

            // Base de piedra (Kiso)
            CreateCube(lantern.transform, "Lantern_Base", new Vector3(0f, 0.15f, 0f), new Vector3(0.9f, 0.3f, 0.9f), mats.stoneGray);
            // Poste central (Sao)
            CreateCylinder(lantern.transform, "Lantern_Post", new Vector3(0f, 0.7f, 0f), new Vector3(0.35f, 0.55f, 0.35f), mats.stoneGray);
            // Plataforma intermedia (Chudai)
            CreateCube(lantern.transform, "Lantern_MidDeck", new Vector3(0f, 1.3f, 0f), new Vector3(0.85f, 0.15f, 0.85f), mats.stoneGray);
            // Caja de luz (Hibukuro)
            GameObject flameBox = CreateCube(lantern.transform, "Lantern_FlameCore", new Vector3(0f, 1.6f, 0f), new Vector3(0.45f, 0.45f, 0.45f), mats.lanternFlame);
            // Tejado tradicional hexagonal/piramidal (Kasa)
            CreateCube(lantern.transform, "Lantern_Roof", new Vector3(0f, 1.95f, 0f), new Vector3(1.1f, 0.2f, 1.1f), mats.stoneGray);
            // Remate superior sagrado (Hoju)
            CreateSphere(lantern.transform, "Lantern_Jewel", new Vector3(0f, 2.15f, 0f), new Vector3(0.3f, 0.3f, 0.3f), mats.stoneGray);

            // Luz cálida interior
            GameObject lightGO = new GameObject("Lantern_PointLight");
            lightGO.transform.SetParent(lantern.transform, false);
            lightGO.transform.position = new Vector3(0f, 1.6f, 0f);
            Light pLight = lightGO.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = new Color(1.0f, 0.78f, 0.42f);
            pLight.intensity = isInteractive ? 0f : 2.0f; // Si es interactivo arranca apagado para encenderlo
            pLight.range = 6.5f;

            if (isInteractive)
            {
                var boxCol = lantern.AddComponent<BoxCollider>();
                boxCol.size = new Vector3(1.1f, 2.2f, 1.1f);
                boxCol.center = new Vector3(0f, 1.1f, 0f);

                var sInt = lantern.AddComponent<XRSimpleInteractable>();
                lantern.AddComponent<ObjectHighlight>();

                var selMgr = lantern.AddComponent<ObjectSelectionManager>();
                SetSelectionManagerData(selMgr, "Farol Tōrō Tradicional", "Farol de piedra ancestral utilizado para guiar los pasos de los caminantes hacia la iluminación.", "• Apunta y presiona Trigger para encender o apagar la llama interior.");

                var lInt = lantern.AddComponent<LanternInteraction>();
                SerializedObject soL = new SerializedObject(lInt);
                soL.FindProperty("lanternLight").objectReferenceValue = pLight;
                soL.FindProperty("fireFlameRenderer").objectReferenceValue = flameBox.GetComponent<MeshRenderer>();
                soL.FindProperty("toggleClip").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Button Pop.wav");
                soL.ApplyModifiedProperties();
            }
        }

        private static void BuildTsukubaiWaterGarden(Transform root, ZenMaterials mats)
        {
            GameObject waterZone = new GameObject("Zone5_TsukubaiWaterPond");
            waterZone.transform.SetParent(root, false);
            waterZone.transform.position = new Vector3(-8f, 0f, 12f);

            // Estanque de piedras naturales
            GameObject pondBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pondBase.name = "Pond_WaterSurface";
            pondBase.transform.SetParent(waterZone.transform, false);
            pondBase.transform.position = new Vector3(0f, 0.1f, 0f);
            pondBase.transform.localScale = new Vector3(6f, 0.05f, 6f);
            pondBase.GetComponent<MeshRenderer>().sharedMaterial = mats.waterPond;
            pondBase.AddComponent<WaterController>();

            // Rocas de borde protectoras del estanque
            for (int a = 0; a < 360; a += 35)
            {
                float rad = a * Mathf.Deg2Rad;
                Vector3 rPos = new Vector3(Mathf.Cos(rad) * 3.1f, 0.25f, Mathf.Sin(rad) * 3.1f);
                CreateSphere(waterZone.transform, $"PondBorderRock_{a}", rPos, new Vector3(0.9f, 0.7f, 0.9f), mats.stoneGray);
            }

            // INTERACCIÓN ESPECIAL 4: Fuente de Agua Tsukubai de Bambú (Shishi-odoshi)
            GameObject tsukubai = new GameObject("Interactive_TsukubaiFountain");
            tsukubai.transform.SetParent(waterZone.transform, false);
            tsukubai.transform.position = new Vector3(0f, 0f, -0.5f);

            // Pileta de piedra ahuecada
            CreateCylinder(tsukubai.transform, "Stone_Basin", new Vector3(0f, 0.4f, 0f), new Vector3(1.2f, 0.4f, 1.2f), mats.stoneGray);

            // Soporte de bambú
            CreateCylinder(tsukubai.transform, "Bamboo_VerticalPost", new Vector3(0.5f, 0.7f, 0.4f), new Vector3(0.12f, 0.7f, 0.12f), mats.bamboo);

            // Caña de bambú basculante
            GameObject lever = CreateCylinder(tsukubai.transform, "Bamboo_WaterSpout", new Vector3(0.2f, 0.9f, 0.1f), new Vector3(0.08f, 0.5f, 0.08f), mats.bamboo, Quaternion.Euler(75f, -30f, 0f));

            // Partículas de chorro de agua y ondas
            ParticleSystem streamPS = CreateWaterStreamParticles(lever.transform);
            ParticleSystem ripplesPS = CreateWaterRippleParticles(tsukubai.transform);

            // Audio del chorro de agua
            AudioSource aSrc = tsukubai.AddComponent<AudioSource>();
            aSrc.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Button Pop.wav");
            aSrc.spatialBlend = 1f;

            var boxCol = tsukubai.AddComponent<BoxCollider>();
            boxCol.size = new Vector3(1.4f, 1.5f, 1.4f);
            boxCol.center = new Vector3(0f, 0.6f, 0f);

            var sInt = tsukubai.AddComponent<XRSimpleInteractable>();
            tsukubai.AddComponent<ObjectHighlight>();

            var selMgr = tsukubai.AddComponent<ObjectSelectionManager>();
            SetSelectionManagerData(selMgr, "Fuente Tsukubai Tradicional", "Fuente ritual utilizada para la purificación espiritual antes de la ceremonia del té.", "• Apunta y presiona Trigger para abrir o cerrar el flujo del agua.");

            var fInt = tsukubai.AddComponent<FountainInteraction>();
            SerializedObject soF = new SerializedObject(fInt);
            soF.FindProperty("waterStreamParticles").objectReferenceValue = streamPS;
            soF.FindProperty("waterRippleParticles").objectReferenceValue = ripplesPS;
            soF.FindProperty("waterAudioSource").objectReferenceValue = aSrc;
            soF.FindProperty("bambooLever").objectReferenceValue = lever.transform;
            soF.ApplyModifiedProperties();
        }

        private static void BuildContemplationTeaPavilion(Transform root, ZenMaterials mats)
        {
            GameObject pavilion = new GameObject("Zone6_ContemplationPavilion");
            pavilion.transform.SetParent(root, false);
            pavilion.transform.position = new Vector3(0f, 0f, 18f);

            // Plataforma de contemplación elevada transitables
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = "Pavilion_Platform";
            platform.transform.SetParent(pavilion.transform, false);
            platform.transform.position = new Vector3(0f, 0.3f, 0f);
            platform.transform.localScale = new Vector3(8f, 0.6f, 6f);
            platform.GetComponent<MeshRenderer>().sharedMaterial = mats.darkWood;

            var tpPlat = platform.AddComponent<TeleportationArea>();
            tpPlat.matchOrientation = MatchOrientation.WorldSpaceUp;

            // 4 Columnas de madera
            CreateCylinder(pavilion.transform, "Col_FL", new Vector3(-3.5f, 2.0f, -2.5f), new Vector3(0.25f, 1.7f, 0.25f), mats.darkWood);
            CreateCylinder(pavilion.transform, "Col_FR", new Vector3(3.5f, 2.0f, -2.5f), new Vector3(0.25f, 1.7f, 0.25f), mats.darkWood);
            CreateCylinder(pavilion.transform, "Col_BL", new Vector3(-3.5f, 2.0f, 2.5f), new Vector3(0.25f, 1.7f, 0.25f), mats.darkWood);
            CreateCylinder(pavilion.transform, "Col_BR", new Vector3(3.5f, 2.0f, 2.5f), new Vector3(0.25f, 1.7f, 0.25f), mats.darkWood);

            // Tejado pagoda japonés
            CreateCube(pavilion.transform, "Roof_Base", new Vector3(0f, 3.8f, 0f), new Vector3(9.2f, 0.3f, 7.2f), mats.darkWood);
            CreateCube(pavilion.transform, "Roof_Peak", new Vector3(0f, 4.4f, 0f), new Vector3(7.5f, 0.5f, 5.5f), mats.stoneGray);

            // Bancos de contemplación Zen
            CreateCube(pavilion.transform, "Contemplation_Bench_L", new Vector3(-2.5f, 0.8f, 0f), new Vector3(1.0f, 0.45f, 3.5f), mats.darkWood);
            CreateCube(pavilion.transform, "Contemplation_Bench_R", new Vector3(2.5f, 0.8f, 0f), new Vector3(1.0f, 0.45f, 3.5f), mats.darkWood);

            // Mesa ceremonial central con cuenco de té
            CreateCylinder(pavilion.transform, "Tea_Table", new Vector3(0f, 0.85f, 0f), new Vector3(1.2f, 0.35f, 1.2f), mats.darkWood);
            CreateSphere(pavilion.transform, "Tea_Bowl", new Vector3(0f, 1.25f, 0f), new Vector3(0.35f, 0.25f, 0.35f), mats.stoneGray);
        }

        private static void BuildBambooAndFlora(Transform root, ZenMaterials mats)
        {
            GameObject floraGroup = new GameObject("Zen_Vegetation_BambooAndSakura");
            floraGroup.transform.SetParent(root, false);

            // Bosque de cañas de bambú japonés perimetral
            for (int i = 0; i < 22; i++)
            {
                float x = (i % 2 == 0 ? -12f - (i * 0.3f) : 12f + (i * 0.3f));
                float z = -8f + (i * 1.5f);
                CreateCylinder(floraGroup.transform, $"BambooStalk_{i}", new Vector3(x, 3.5f, z), new Vector3(0.18f, 3.5f, 0.18f), mats.bamboo);
            }

            // Árboles de Sakura grandes en las esquinas
            CreateSakuraTree(floraGroup.transform, new Vector3(-9f, 0f, -7f), mats);
            CreateSakuraTree(floraGroup.transform, new Vector3(9f, 0f, 8f), mats);
            CreateSakuraTree(floraGroup.transform, new Vector3(-9f, 0f, 19f), mats);
        }

        private static void CreateSakuraTree(Transform parent, Vector3 pos, ZenMaterials mats)
        {
            GameObject tree = new GameObject("SakuraTree_Kyoto");
            tree.transform.SetParent(parent, false);
            tree.transform.position = pos;

            // Tronco
            CreateCylinder(tree.transform, "Trunk", new Vector3(0f, 2.5f, 0f), new Vector3(0.6f, 2.5f, 0.6f), mats.darkWood);
            // Ramas y follaje rosa
            CreateSphere(tree.transform, "Foliage_Center", new Vector3(0f, 5.2f, 0f), new Vector3(3.8f, 2.5f, 3.8f), mats.sakuraPink);
            CreateSphere(tree.transform, "Foliage_North", new Vector3(0f, 4.8f, 1.2f), new Vector3(2.5f, 2.0f, 2.5f), mats.sakuraPink);
            CreateSphere(tree.transform, "Foliage_South", new Vector3(0f, 4.8f, -1.2f), new Vector3(2.5f, 2.0f, 2.5f), mats.sakuraPink);
        }
        #endregion

        #region Interfaz World Space Zen
        private static void SetupZenWorldSpaceUI(Transform root, ZenMaterials mats)
        {
            GameObject billboard = new GameObject("Zen_InstructionBoard_WorldSpace");
            billboard.transform.SetParent(root, false);
            billboard.transform.position = new Vector3(2.8f, 1.6f, -10f);
            billboard.transform.rotation = Quaternion.Euler(0f, -30f, 0f);

            // Postes de madera oscura
            CreateCylinder(billboard.transform, "Post_L", new Vector3(-1.3f, -0.6f, 0f), new Vector3(0.12f, 1.0f, 0.12f), mats.darkWood);
            CreateCylinder(billboard.transform, "Post_R", new Vector3(1.3f, -0.6f, 0f), new Vector3(0.12f, 1.0f, 0.12f), mats.darkWood);

            // Tablón de soporte
            CreateCube(billboard.transform, "Wood_Frame", Vector3.zero, new Vector3(2.8f, 1.8f, 0.1f), mats.darkWood);
            CreateCube(billboard.transform, "Board_Backing", new Vector3(0f, 0f, 0.04f), new Vector3(2.6f, 1.6f, 0.05f), mats.sand);

            // Canvas World Space
            GameObject canvasGO = new GameObject("WorldSpace_Canvas");
            canvasGO.transform.SetParent(billboard.transform, false);
            canvasGO.transform.localPosition = new Vector3(0f, 0f, 0.08f);
            canvasGO.transform.localRotation = Quaternion.identity;

            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            RectTransform rt = canvasGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(900f, 600f);
            rt.localScale = new Vector3(0.0028f, 0.0026f, 0.0028f);

            // Textos estilizados
            TextMeshProUGUI title = CreateText(canvasGO.transform, "TitleText", "JARDÍN ZEN DE KIOTO", 42f, TextAlignmentOptions.Center, new Vector2(0f, 230f), new Vector2(850f, 80f), new Color(0.2f, 0.14f, 0.1f));
            TextMeshProUGUI desc = CreateText(canvasGO.transform, "DescText", "Sumérgete en la contemplación, descubre los cuatro elementos sagrados del jardín y experimenta su serenidad.", 24f, TextAlignmentOptions.Center, new Vector2(0f, 120f), new Vector2(820f, 120f), new Color(0.3f, 0.22f, 0.16f));
            TextMeshProUGUI actions = CreateText(canvasGO.transform, "ActionsText", "• Teletransporte: Apunta al sendero o césped y suelta el gatillo.\n• Selección: Apunta a los objetos con el rayo para descubrir su historia.", 22f, TextAlignmentOptions.TopLeft, new Vector2(0f, -10f), new Vector2(800f, 100f), new Color(0.25f, 0.2f, 0.15f));
            TextMeshProUGUI counter = CreateText(canvasGO.transform, "CounterText", "Elementos en Armonía: 0 / 4", 28f, TextAlignmentOptions.Center, new Vector2(0f, -130f), new Vector2(800f, 60f), new Color(0.55f, 0.2f, 0.15f));
            TextMeshProUGUI notify = CreateText(canvasGO.transform, "NotificationText", "", 24f, TextAlignmentOptions.Center, new Vector2(0f, -210f), new Vector2(820f, 60f), new Color(0.18f, 0.42f, 0.22f));

            // Componente VRInteractionUI
            var uiCtrl = canvasGO.AddComponent<VRInteractionUI>();
            SerializedObject soUI = new SerializedObject(uiCtrl);
            soUI.FindProperty("titleText").objectReferenceValue = title;
            soUI.FindProperty("descriptionText").objectReferenceValue = desc;
            soUI.FindProperty("actionText").objectReferenceValue = actions;
            soUI.FindProperty("counterText").objectReferenceValue = counter;
            soUI.FindProperty("notificationText").objectReferenceValue = notify;
            soUI.ApplyModifiedProperties();
        }

        private static TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, TextAlignmentOptions align, Vector2 pos, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.color = color;
            tmp.enableWordWrapping = true;
            return tmp;
        }
        #endregion

        #region Controladores de Atmósfera
        private static void SetupZenAtmosphereControllers(Transform root)
        {
            GameObject atm = new GameObject("Zen_Atmosphere_Managers");
            atm.transform.SetParent(root, false);

            // Controlador de efectos ambientales (brisa y pétalos de sakura cayendo)
            ParticleSystem globalSakuraPS = CreateSakuraPetalParticles(atm.transform);
            var envEffects = atm.AddComponent<EnvironmentEffects>();
            SerializedObject soE = new SerializedObject(envEffects);
            soE.FindProperty("fallingSakuraPetals").objectReferenceValue = globalSakuraPS;
            soE.ApplyModifiedProperties();

            // Controlador de audio ambiental
            AudioSource aNat = atm.AddComponent<AudioSource>();
            aNat.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Button Pop.wav");
            aNat.loop = true;
            aNat.playOnAwake = true;

            var aCtrl = atm.AddComponent<AmbientAudioController>();
            SerializedObject soA = new SerializedObject(aCtrl);
            soA.FindProperty("ambientNatureSource").objectReferenceValue = aNat;
            soA.ApplyModifiedProperties();
        }
        #endregion

        #region Helpers Primitivos y Partículas
        private static GameObject CreateCube(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            if (mat != null) go.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return go;
        }

        private static GameObject CreateCylinder(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat, Quaternion? rot = null)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            if (rot.HasValue) go.transform.localRotation = rot.Value;
            if (mat != null) go.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return go;
        }

        private static GameObject CreateSphere(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            if (mat != null) go.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return go;
        }

        private static void SetSelectionManagerData(ObjectSelectionManager mgr, string name, string desc, string actions)
        {
            SerializedObject so = new SerializedObject(mgr);
            so.FindProperty("objectName").stringValue = name;
            so.FindProperty("objectDescription").stringValue = desc;
            so.FindProperty("availableActions").stringValue = actions;
            so.ApplyModifiedProperties();
        }

        private static ParticleSystem CreateSakuraPetalParticles(Transform parent)
        {
            GameObject psGO = new GameObject("Particles_SakuraPetals");
            psGO.transform.SetParent(parent, false);
            psGO.transform.localPosition = new Vector3(0f, 4f, 0f);

            ParticleSystem ps = psGO.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 6f;
            main.startSpeed = 0.8f;
            main.startSize = 0.12f;
            main.startColor = new Color(0.95f, 0.78f, 0.85f, 0.85f);
            main.gravityModifier = 0.05f;

            var emission = ps.emission;
            emission.rateOverTime = 8f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(6f, 1f, 6f);
            return ps;
        }

        private static ParticleSystem CreatePlacementParticles(Transform parent)
        {
            GameObject psGO = new GameObject("Particles_ZenPlacement");
            psGO.transform.SetParent(parent, false);
            psGO.transform.localPosition = Vector3.up * 0.1f;

            ParticleSystem ps = psGO.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 1.8f;
            main.startSpeed = 1.2f;
            main.startSize = 0.15f;
            main.startColor = new Color(1.0f, 0.9f, 0.5f, 0.9f);
            main.playOnAwake = false;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 25) });
            return ps;
        }

        private static ParticleSystem CreateWaterStreamParticles(Transform parent)
        {
            GameObject psGO = new GameObject("Particles_WaterStream");
            psGO.transform.SetParent(parent, false);
            psGO.transform.localPosition = Vector3.forward * 0.4f;

            ParticleSystem ps = psGO.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 0.8f;
            main.startSpeed = 1.5f;
            main.startSize = 0.06f;
            main.startColor = new Color(0.6f, 0.85f, 0.95f, 0.7f);
            main.gravityModifier = 1.0f;

            var emission = ps.emission;
            emission.rateOverTime = 20f;
            return ps;
        }

        private static ParticleSystem CreateWaterRippleParticles(Transform parent)
        {
            GameObject psGO = new GameObject("Particles_WaterRipples");
            psGO.transform.SetParent(parent, false);
            psGO.transform.localPosition = new Vector3(0f, 0.42f, 0f);

            ParticleSystem ps = psGO.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 1.2f;
            main.startSpeed = 0.3f;
            main.startSize = 0.4f;
            main.startColor = new Color(0.8f, 0.95f, 1.0f, 0.5f);

            var emission = ps.emission;
            emission.rateOverTime = 4f;
            return ps;
        }
        #endregion
    }
}
