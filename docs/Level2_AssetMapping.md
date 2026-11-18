# Ponyo — Mapping des assets du niveau 2

**Mapping validé.** Source de conception : [PONYO_NIVEAU_2_SPEC.md](PONYO_NIVEAU_2_SPEC.md). Les choix couvrent les quatre sections. Le layout construit uniquement la section 1. L’inventaire complet des modèles est en fin de document.

## Lecture du mapping

Dans les tableaux, `mini-forest_1.0/rocks-low` désigne le fichier `3d_assets/kenney_mini-forest_1.0/Models/FBX format/rocks-low.fbx`. Tous les raccourcis de modèles suivent cette convention. Les FBX sont des **sources de visuels** à importer lors d’une étape ultérieure. Le dossier `3d_assets/` contient **687 modèles** répartis sur 11 kits, les mêmes noms en FBX, GLB et OBJ, un pack de 5 skyboxes et **0 fichier `.prefab`**. Les prefabs de gameplay cités se trouvent déjà dans `Assets/Prefabs/Level2/`.

Le rendu utilise des meshes 3D avec une caméra de profil côté `−Z`. `+X` est le sens de progression et `+Y` la verticale. Les visuels directionnels sont orientés sur un parent, vers `+X` pour les marches et les poissons. La voiture arrivera de la droite et fera face à `−X`. Le parent du visuel porte aussi l’échelle, ajustée aux dimensions ci-dessous. Le FBX garde sa transformation native. Les colliders 2D sont indépendants du visuel. Les assets `*_MVP.png` restent disponibles comme placeholders temporaires.

## Échelles cibles

Le layout définit un zoom caméra de 1,2 et un décalage côtier de -0,348 unité. Le sol plat est à Y = 1,152 et la mer à Y = -0,24, soit une hauteur émergée de 1,392 unité. L'escalier, les obstacles, les décors proches et les points de départ suivent ce décalage. Les réglages sont enregistrés dans `Level2Layout` et repris par le générateur.

Les dimensions sont données en unités Unity, dans l’ordre largeur X × hauteur Y × profondeur Z, après rotation du visuel. Pour les décors répétés, le layout peut appliquer une variation supplémentaire sur le parent du prefab.

| Élément | Taille cible (X × Y × Z) | État |
| --- | --- | --- |
| Ponyo | Hitbox 2D de 0,75 × 1,15 | Capsule rouge de remplacement |
| Escalier de profil | 8,80 × 2,90 × 4,10 | Section 1, fondation incluse |
| Sol côtier et couche d'herbe | 73,60 × 3,00 × 4,13 | Section 1 prolongée |
| Rocher / coquillage | 1,14 × 0,54 × 0,85 / 0,70 × 0,25 × 0,42 | Section 1 |
| Bulle / flaque | 0,50 × 0,50 × 0,50 / 3,35 × 0,12 × 1,80 | Section 1 |
| Petite vague / poisson | 2,85 × 0,42 × 1,20 / 1,65 × 0,62 × 0,38 | Section 1 |
| Buisson / arbuste / touffe d’herbe | 1,30 × 0,65 × 0,85 / 0,82 × 1,24 × 0,75 / 0,58 × 0,35 × 0,45 | Section 1 |
| Fleur / petites pierres | env. 0,31 × 0,44 × 0,14 / 0,72 × 0,24 × 0,48 | Section 1 |
| Plaque de terre / plaque d’herbe | 1,52 × 0,06 × 1,00 / 1,62 × 0,14 × 0,85 | Section 1 |
| Tuile d’eau / tuile d’eau avec roches | 10,50 × 0,40 × 9,00 / 10,50 × 1,25 × 9,00 | Section 1 |
| Bouée | 0,75 × 1,10 × 0,75 | Section 1 |
| Île proche / île lointaine | 7,00 × 1,60 × 5,60 / 10,00 × 3,80 × 6,00 | Section 1 |
| Rocher de fond / arbre | 2,60 × 1,72 × 1,80 / 1,10 × 2,10 × 1,06 | Section 1 |
| Falaise en coupe | 6,0 × 4,0 × 3,0 par segment | Cible section 2 |
| Barrière / ronces / poteau électrique | 1,60 × 0,85 × 0,45 / 1,20 × 0,55 × 0,75 / 0,80 × 4,40 × 0,80 | Cibles section 2 |
| Rafale / icône d’alerte | 1,40 × 2,10 × 0,80 / 0,60 × 0,60 × 0,08 | Cibles sections 2–3 |
| Route / zone surélevée | 8,0 × 0,45 × 4,5 par module / 2,80 × 0,65 × 2,40 | Cibles section 3 |
| Maison / lampadaire / voiture | 5,0 × 3,6 × 3,6 / 0,75 × 3,20 × 0,75 / 2,65 × 1,15 × 1,45 | Cibles sections 3–4 |
| Faisceau / seconde vague-poisson | 4,0 × 0,55 × 0,55 par phare / 3,40 × 0,62 × 1,45 | Cibles section 3 |
| Caisse flottante / caisse fragile / barque | 2,10 × 0,66 × 1,30 / 1,35 × 0,65 × 1,10 / 3,20 × 0,85 × 1,65 | Cibles section 4 |
| Crabe / planches / tonneau | 0,65 × 0,40 × 0,50 / 1,65 × 0,28 × 0,80 / 0,75 × 0,80 × 0,75 | Cibles section 4 |
| Jet d’eau / zone sûre / colline finale | 0,70 × 2,80 × 0,70 / 2,50 × 0,70 × 2,0 / 12,0 × 5,0 × 6,0 | Cibles section 4 |
| Repère de checkpoint / repère de fin | 0,45 × 1,20 × 0,45 / 1,6 × 2,2 × 0,25 | Cibles des visuels de progression |

Le ciel, le brouillard et les éclaboussures se règlent par les paramètres de rendu et d’émission. Ils ne constituent pas des prefabs à dimension fixe.

## Section 1 - Sortie de l'eau et approche urbaine

La section s'étend jusqu'à X = 82,4. Les 18,4 unités ajoutées représentent 5 secondes à 3,68 unités/s. À partir de X = 59, le chemin de terre s'élargit progressivement. Dix segments de glissière blanche à réflecteurs et deux lampadaires amorcent le passage vers la ville. Un rocher à X = 67,4, un coquillage à X = 71,2 et une bulle à X = 76,5 prolongent la séquence d'obstacles. Le checkpoint et l'arrêt de prévisualisation se trouvent à X = 80,2 et X = 81,8.

| Élément | Modèle Kenney choisi ou placeholder proposé | Placement et réemploi Unity |
| --- | --- | --- |
| Escalier sortant de la mer | `building-kit/stairs-closed` et `SurfaceTextures/CoastalStairStone.jpg` | De profil, montant vers `+X`. La texture de pavés gris fournie couvre le côté, les marches, les contremarches et le palier. La fondation descend jusqu'au bas de la terre. Un `EdgeCollider2D` continu permet de monter en course automatique. |
| Palier entre l'escalier et la terre | `CoastalStairLanding`, pierre moussue, `mini-forest_1.0/stones` et `survival-kit/grass-large` | Le dallage se prolonge dans l'herbe et les pierres se terminent en quinconce sur la coupe de terre. Le palier est décoratif. |
| Terrain de sortie, terre et herbe | `mini-forest_1.0/patch-dirt` et `mini-forest_1.0/patch-grass` en habillage | Corps et couche d’herbe en meshes 3D. Le même `EdgeCollider2D` se prolonge de l’escalier au sol plat. |
| Rochers à sauter | `mini-forest_1.0/rocks-low` | Le prefab `Rock_3D` reprend le collider solide du blockout et l’ajuste au modèle. |
| Coquillages | **MANQUANT** · `Shell_PLACEHOLDER`, sphère aplatie beige rosé | Réutiliser `Obstacles/Shell_PLACEHOLDER.prefab` et son collider solide bas. |
| Bulles de capture | **MANQUANT** · `BubbleTrap_PLACEHOLDER`, sphère cyan translucide avec reflet blanc | Réutiliser `Obstacles/BubbleTrap_PLACEHOLDER.prefab` et `BubbleTrap`. Bulle isolée pour la première rencontre. |
| Flaques profondes | **MANQUANT** · `DeepPuddle_PLACEHOLDER`, sphère aplatie bleu foncé | Réutiliser `Obstacles/DeepPuddle_PLACEHOLDER.prefab` et `PuddleSlowZone`. Collider en trigger pour le ralentissement. |
| Petite vague-poisson | **MANQUANT** pour la vague · `FishWave_PLACEHOLDER`, ellipsoïde turquoise surmonté de `survival-kit/fish` teinté | Réutiliser `Platforms/FishWave_PLACEHOLDER.prefab`, `BounceSurface2D` et `PingPongMover2D`. L’atterrissage déclenche le rebond. |
| Buissons | `mini-forest_1.0/plant` | Regrouper des exemplaires à faible hauteur près des marches et du chemin. |
| Arbustes | `city-kit-suburban_20/tree-small` | Réduire l’échelle pour les distinguer des arbres. |
| Arbres et végétation côtière | `mini-forest_1.0/tree`, `mini-forest_1.0/plant` | Arbres sur les îles et en retrait du parcours. Plantes près des marches. |
| Fleurs | **MANQUANT** · `Flower_PLACEHOLDER`, petites sphères rose et jaune sur cylindre vert | Décor proche en petites grappes. |
| Touffes d'herbe | `GrassTuft` | Brins 3D courbés, verts nuancés et vent léger sur les pointes. Les marches inférieures sont dégagées. Trois touffes restent sur la dernière marche haute, avec les plantes du palier. |
| Petites pierres décoratives | `mini-forest_1.0/stones` | Utiliser une échelle et une teinte différentes du rocher à franchir. |
| Mer proche, mer lointaine et horizon | `hexagon-kit/water`, `hexagon-kit/water-rocks` et mouvement extrait de `3d_assets/water_waves.glb` | Le niveau moyen est à `y = -0,24`. Le relief animé utilise 128 échantillons du modèle sur 16,67 secondes, avec des raccords spatiaux et temporels adaptés à la mer répétée. Le matériau reprend une eau bleu-vert brillante et reflète le ciel. Le mouvement diminue près du rivage. Seize petits émetteurs produisent ponctuellement 2 à 4 gouttelettes au contact des marches et du sol. |
| Bouées | `watercraft-pack/buoy` | Décor intermédiaire sans collision. |
| Clôtures en bois | `survival-kit/fence` | Cinq tronçons sans collision, au bord arrière du terrain ferme. |
| Bosquets et herbes | `mini-forest_1.0/tree-high`, `mini-forest_1.0/plant`, `city-kit-suburban_20/tree-small`, `CoastalMeadow` | Arbres et arbustes en retrait, touffes d'herbe animées au bord du chemin et de l'escalier. Le vent se règle dans `CoastalGrass.mat`. |
| Îles au large et au plan intermédiaire | `hexagon-kit/stone-hill`, `hexagon-kit/grass-hill`, `hexagon-kit/grass-forest`, rochers `mini-forest_1.0` | Caps, criques boisées et îlots rocheux. Bosquets mêlant `mini-forest_1.0/tree` et `plant`, `city-kit-suburban_20/tree-large`, `survival-kit/tree` et `tree-tall`, `CoastalMeadow` et les fleurs roses du niveau. Les plantes suivent les surfaces du relief. Les îles et la mer sont ancrées dans le monde. |

**Ordre du tutoriel.** Escalier et introduction à l’abri, rocher isolé, coquillage bas, bulle seule, flaque, puis vague-poisson près de la sortie. Cet ordre reprend le blockout actuel et respecte la progression de la spec. Les repères existants (`x = 18, 26, 34, 42, 54`) servent de point de départ pour régler les espacements après essai des sauts.

## Section 2 — Falaise végétalisée (20–45 s)

| Élément | Modèle Kenney choisi ou placeholder proposé | Placement et interaction |
| --- | --- | --- |
| Chemin côtier terre et herbe | `mini-forest_1.0/patch-dirt` et `mini-forest_1.0/patch-grass` | Habillage du sol continu de la section 1. |
| Falaise en coupe et relief côtier | `mini-forest_1.0/rocks-high` | Habillage sous le chemin et en arrière-plan. Préserver une surface jouable continue en haut. |
| Eau visible sous la falaise | `hexagon-kit/water` et `hexagon-kit/water-rocks` | Bande d’eau décorative sous le terrain. |
| Clôture en bois décorative | `survival-kit/fence` | En retrait du plan jouable. |
| Barrière à sauter | `survival-kit/fence` | Variante plus contrastée sur le plan jouable, avec `BoxCollider2D` solide. |
| Ronces | **MANQUANT** · `Brambles_PLACEHOLDER`, sphères vert sombre et petits cubes violets tournés en pointe | Obstacle bas. Trigger 2D de retour au checkpoint. |
| Poteau électrique et câbles | **MANQUANT** · `UtilityPole_PLACEHOLDER`, cylindre brun, traverse en cube et câbles en cylindres noirs fins | Décor intermédiaire. |
| Bulles de capture | **MANQUANT** · même `BubbleTrap_PLACEHOLDER` que section 1 | Réemploi du prefab avec un espacement plus exigeant. |
| Rafale, feuilles soulevées, alerte et stun | **MANQUANT** · `WindGust_PLACEHOLDER` en quads blancs translucides, `WindLeaves_PLACEHOLDER` en petits quads verts, `HazardWarning_PLACEHOLDER` en losange ambre, `StunEffect_PLACEHOLDER` en sphères jaunes | Alerte à droite, trigger de rafale, rotation visuelle et perte de contrôle d’environ 1 s. Première rencontre dans une zone dégagée. |

## Section 3 — Route sous l’orage (45–70 s)

| Élément | Modèle Kenney choisi ou placeholder proposé | Placement et interaction |
| --- | --- | --- |
| Route goudronnée | `toy-car-kit/track-road-wide-straight` | Surface anthracite en segments. Les bordures rouge et blanc sont visibles dans le modèle et pourront être atténuées. Sol continu en collider 2D. |
| Maisons distantes et silhouettes du village | `city-kit-suburban_20/building-type-a` et `city-kit-suburban_20/building-type-b` | Variantes dans le plan intermédiaire. Densifier les maisons à l’approche du village. |
| Clôtures et barrière renversée | `survival-kit/fence` | Version inclinée pour la route. Collider solide uniquement sur l’obstacle jouable. |
| Lampadaire droit ou renversé | `graveyard-kit_5.0/lightpost-single` | Modèle droit en décor. Variante inclinée avec collider 2D simple pour le saut. |
| Voiture rapide | `toy-car-kit/vehicle-suv` et roues séparées `toy-car-kit/wheel-medium` | Orienter vers la gauche, un seul passage scripté. Trigger 2D de danger. |
| Pleins phares | **MANQUANT** · `HeadlightBeams_PLACEHOLDER`, deux quads translucides jaune pâle inclinés vers la gauche | Afficher avant que la voiture devienne dangereuse. |
| Zone surélevée pour esquiver | `building-kit/floor-half` | Visuel de surélévation. Le collider définit la surface d’atterrissage. |
| Seconde vague-poisson | **MANQUANT** pour la vague · `FishWaveLarge_PLACEHOLDER`, ellipsoïde bleu avec `survival-kit/fish-large` teinté | Variante du prefab de section 1, timing plus exigeant. |
| Rafales et avertissement | **MANQUANT** · mêmes placeholders que section 2 | Réutiliser les visuels et le principe d’avertissement. |

## Section 4 — Village submergé (70–105 s)

| Élément | Modèle Kenney choisi ou placeholder proposé | Placement et interaction |
| --- | --- | --- |
| Maisons, clôtures et éléments urbains inondés | `city-kit-suburban_20/building-type-a`, `city-kit-suburban_20/building-type-b`, `survival-kit/fence`, `graveyard-kit_5.0/lightpost-single` | Visuels réutilisés et partiellement immergés. |
| Eau montante dangereuse et surface agitée | `hexagon-kit/water` et `hexagon-kit/water-rocks` | Bandes 3D pilotées par la hauteur d’eau, trigger 2D de danger et éclaboussures en `ParticleSystem`. |
| Caisse flottante | `survival-kit/box-large` | Plateforme en `BoxCollider2D` dont la hauteur suit l’eau. |
| Caisse fragile | `survival-kit/box` | Teinte plus claire et fissures en quads sombres. Effet de casse séparé après environ 1 s. |
| Barque flottante | `watercraft-pack/boat-row-large` | Assez de place pour distinguer l’appui jouable de la zone occupée par le crabe. |
| Crabe | **MANQUANT** · `Crab_PLACEHOLDER`, sphère rouge aplatie et deux petites sphères rouges pour les pinces | Danger distinct de la plateforme de la barque. |
| Débris et autres appuis flottants | `survival-kit/resource-planks` et `survival-kit/barrel` | Planches pour les appuis ponctuels, tonneau pour les débris décoratifs. |
| Vagues-poissons | **MANQUANT** pour la vague · réemploi de `FishWave_PLACEHOLDER` et `FishWaveLarge_PLACEHOLDER` | Appuis rebondissants pour la montée finale. |
| Jet d’eau | **MANQUANT** · `WaterJet_PLACEHOLDER`, cylindre cyan translucide et petites sphères pour l’annonce | Prototype proposé vertical, annoncé, avec impulsion vers le haut. |
| Zones sûres temporaires | `hexagon-kit/grass` | Petites plateformes fixes émergées entre deux séquences. |
| Colline finale | `hexagon-kit/grass-hill` et `survival-kit/rock-flat-grass` | Relief en habillage et sommet sec sur collider stable. Trigger de fin au sommet. |

## Personnage, météo, progression et sons

| Besoin | Modèle Kenney choisi ou placeholder proposé |
| --- | --- |
| Personnage jouable | Capsule rouge avec collider 2D et contrôleur de déplacement. |
| Ciel clair, nuageux et orageux | Skybox panoramique à partir de `3d_assets/kenney_skyboxes/Skyboxes/skybox-day.png`. **MANQUANT** pour un panorama d’orage dédié. `WeatherDirector` pilote la skybox, le fog et la lumière directionnelle. |
| Pluie légère/forte, brume et éclaboussures | `ParticleSystem` pour la pluie et les éclaboussures, fog pour la brume. Les émetteurs météo seront placés dans les sections où la tempête commence. |
| Éclats de caisse | **MANQUANT** · `CrateDebris_PLACEHOLDER`, petits cubes bruns. |
| Checkpoints et signal de fin | **MANQUANT** pour les marqueurs dédiés · `Checkpoint_PLACEHOLDER`, cube vert, et `LevelEnd_PLACEHOLDER`, anneau doré formé de sphères. Le prefab visuel proposé reste indépendant des triggers. |
| Sons : mer, oiseaux, pas, sauts/rebonds, capture, annonce et impact du vent, stun, pluie, moteur/klaxon, vague-poisson, bois/caisse, crue et fin | **MANQUANT** dans les packs inventoriés · événements sonores silencieux pour le blockout. |

## Réemploi et décisions de prototype

- `Tools > Ponyo > Build Level 2` ouvre ou réutilise `Assets/Scenes/Level2_Ponyo.unity` et lit `Assets/Art/Level2/Level2Layout.asset`. Au premier build, il remplace les objets identifiés du blockout de la section 1. Les relances reconstruisent la racine `Generated_Level2`. L’ancien point d’entrée délègue au nouveau générateur.
- Systèmes réutilisés pour la section 1 : `PonyoMovement2D`, `CameraFollow2D`, `ParallaxLayer`, `BubbleTrap`, `CheckpointRespawner`, `PuddleSlowZone`, `BounceSurface2D` et `PingPongMover2D`. Les nouveaux prefabs des obstacles reprennent les composants du blockout avec des visuels 3D et des colliders ajustés.
- Les repères du blockout utilisent 3,2 unités/s. Les longueurs sont de 82,4 / 80 / 80 / 112 unités, soit 354,4 unités et un budget nominal de 110,75 secondes. La course utilise le multiplicateur 1,15, soit 3,68 unités/s. La prolongation de 18,4 unités ajoute donc 5 secondes de course effective. Les ralentissements, sauts et reprises influencent le temps mesuré en jeu.
- Par défaut, une capture déclenche une immobilisation de 0,45 s suivie du retour au checkpoint. L’animation de capture est à prévoir. La section 1 possède un checkpoint de sortie et un arrêt de prévisualisation en bout de terrain. Les futurs checkpoints des sections 2 et 3 sont prévus avec restauration de la météo et de l’eau. La voiture, les ronces et l’eau dangereuse déclencheront un retour au checkpoint.
- Pour le premier prototype, retenir le saut court ou maintenu et différer le plané optionnel. La spec laisse la direction et l’effet du jet ouverts. Le choix proposé ici est un jet vertical annoncé qui projette Ponyo vers le haut.
- Le rapport Graphify correspond au commit `5565c1d6`. Les fichiers de travail du niveau 2 ont été inspectés directement dans `Assets/`.

## Inventaire complet des sources Kenney

Les listes suivantes utilisent les noms de fichiers sans extension. Pour chacun de ces noms, le FBX se trouve dans `Models/FBX format/` du kit concerné. Les trois formats ont été comparés par nom à l’intérieur de chaque kit.

### kenney_building-kit — 79 modèles

- `barricade-doorway-a`, `barricade-doorway-b`, `barricade-doorway-c`, `barricade-window-a`, `barricade-window-b`, `barricade-window-c`, `border-corner-diagonal`, `border-corner-round`
- `border-corner-small`, `border-corner`, `border-high-corner-diagonal`, `border-high-corner-round`, `border-high-corner-small`, `border-high-corner`, `border-high`, `border`
- `column-thin`, `column-wide`, `column`, `detail-pipe`, `door-rotate-round-a`, `door-rotate-round-b`, `door-rotate-round-c`, `door-rotate-round-d`
- `door-rotate-square-a`, `door-rotate-square-b`, `door-rotate-square-c`, `door-rotate-square-d`, `floor-corner-diagonal`, `floor-corner-round`, `floor-half`, `floor-quarter`
- `floor`, `gutter-vertical-bottom`, `gutter-vertical-short`, `gutter-vertical-top`, `gutter-vertical-wall`, `gutter-vertical`, `plating-detailed-wide`, `plating-detailed`
- `plating-wide`, `plating`, `roof-flat-center`, `roof-flat-corner-inner`, `roof-flat-corner`, `roof-flat-patch-large`, `roof-flat-patch`, `roof-flat-side`
- `roof-flat-square`, `stairs-center-short`, `stairs-center`, `stairs-closed-short`, `stairs-closed`, `stairs-open-short`, `stairs-open`, `stairs-sides-short`
- `stairs-sides`, `wall-corner-column-bottom`, `wall-corner-column-small-bottom`, `wall-corner-column-small`, `wall-corner-column`, `wall-corner-diagonal`, `wall-corner-round`, `wall-corner`
- `wall-doorway-round`, `wall-doorway-square`, `wall-doorway-wide-round`, `wall-doorway-wide-square`, `wall-half`, `wall-low`, `wall-window-round-detailed`, `wall-window-round`
- `wall-window-square-detailed`, `wall-window-square`, `wall-window-wide-round-detailed`, `wall-window-wide-round`, `wall-window-wide-square-detailed`, `wall-window-wide-square`, `wall`

### kenney_city-kit-commercial_2.1 — 41 modèles

- `building-a`, `building-b`, `building-c`, `building-d`, `building-e`, `building-f`, `building-g`, `building-h`
- `building-i`, `building-j`, `building-k`, `building-l`, `building-m`, `building-n`, `building-skyscraper-a`, `building-skyscraper-b`
- `building-skyscraper-c`, `building-skyscraper-d`, `building-skyscraper-e`, `detail-awning-wide`, `detail-awning`, `detail-overhang-wide`, `detail-overhang`, `detail-parasol-a`
- `detail-parasol-b`, `low-detail-building-a`, `low-detail-building-b`, `low-detail-building-c`, `low-detail-building-d`, `low-detail-building-e`, `low-detail-building-f`, `low-detail-building-g`
- `low-detail-building-h`, `low-detail-building-i`, `low-detail-building-j`, `low-detail-building-k`, `low-detail-building-l`, `low-detail-building-m`, `low-detail-building-n`, `low-detail-building-wide-a`
- `low-detail-building-wide-b`

### kenney_city-kit-industrial_2.0 — 37 modèles

- `building-a`, `building-b`, `building-c`, `building-d`, `building-e`, `building-f`, `building-g`, `building-h`
- `building-i`, `building-j`, `building-k`, `building-l`, `building-m`, `building-n`, `building-o`, `building-p`
- `building-q`, `building-r`, `building-s`, `building-t`, `chimney-basic`, `chimney-large`, `chimney-medium`, `chimney-small`
- `detail-tank-large`, `detail-tank`, `shipping-container-a`, `shipping-container-b`, `shipping-container-c`, `solar-panel-flat`, `solar-panel-landscape-group`, `solar-panel-landscape`
- `solar-panel-portrait-group`, `solar-panel-portrait`, `water-tower`, `windmill-low`, `windmill`

### kenney_city-kit-suburban_20 — 40 modèles

- `building-type-a`, `building-type-b`, `building-type-c`, `building-type-d`, `building-type-e`, `building-type-f`, `building-type-g`, `building-type-h`
- `building-type-i`, `building-type-j`, `building-type-k`, `building-type-l`, `building-type-m`, `building-type-n`, `building-type-o`, `building-type-p`
- `building-type-q`, `building-type-r`, `building-type-s`, `building-type-t`, `building-type-u`, `driveway-long`, `driveway-short`, `fence-1x2`
- `fence-1x3`, `fence-1x4`, `fence-2x2`, `fence-2x3`, `fence-3x2`, `fence-3x3`, `fence-low`, `fence`
- `path-long`, `path-short`, `path-stones-long`, `path-stones-messy`, `path-stones-short`, `planter`, `tree-large`, `tree-small`

### kenney_graveyard-kit_5.0 — 91 modèles

- `altar-stone`, `altar-wood`, `bench-damaged`, `bench`, `border-pillar`, `brick-wall-curve-small`, `brick-wall-curve`, `brick-wall-end`
- `brick-wall`, `candle-multiple`, `candle`, `character-ghost`, `character-keeper`, `character-skeleton`, `character-vampire`, `character-zombie`
- `coffin-old`, `coffin`, `column-large`, `cross-column`, `cross-wood`, `cross`, `crypt-a`, `crypt-b`
- `crypt-door`, `crypt-large-door`, `crypt-large-roof`, `crypt-large`, `crypt-small-roof`, `crypt-small`, `crypt`, `debris-wood`
- `debris`, `detail-bowl`, `detail-chalice`, `detail-plate`, `fence-damaged`, `fence-gate`, `fence`, `fire-basket`
- `grave-border`, `grave`, `gravestone-bevel`, `gravestone-broken`, `gravestone-cross-large`, `gravestone-cross`, `gravestone-debris`, `gravestone-decorative`
- `gravestone-roof`, `gravestone-round`, `gravestone-wide`, `hay-bale-bundled`, `hay-bale`, `iron-fence-bar`, `iron-fence-border-column`, `iron-fence-border-curve`
- `iron-fence-border-gate`, `iron-fence-border`, `iron-fence-curve`, `iron-fence-damaged`, `iron-fence`, `lantern-candle`, `lantern-glass`, `lightpost-all`
- `lightpost-double`, `lightpost-single`, `pillar-large`, `pillar-obelisk`, `pillar-small`, `pillar-square`, `pine-crooked`, `pine-fall-crooked`
- `pine-fall`, `pine`, `pumpkin-carved`, `pumpkin-tall-carved`, `pumpkin-tall`, `pumpkin`, `road`, `rocks-tall`
- `rocks`, `shovel-dirt`, `shovel`, `stone-wall-column`, `stone-wall-curve`, `stone-wall-damaged`, `stone-wall`, `trunk-long`
- `trunk`, `urn-round`, `urn-square`

### kenney_hexagon-kit — 72 modèles

- `bridge`, `building-archery`, `building-cabin`, `building-castle`, `building-dock`, `building-farm`, `building-house`, `building-market`
- `building-mill`, `building-mine`, `building-port`, `building-sheep`, `building-smelter`, `building-tower`, `building-village`, `building-wall`
- `building-walls`, `building-watermill`, `building-wizard-tower`, `dirt-lumber`, `dirt`, `grass-forest`, `grass-hill`, `grass`
- `path-corner-sharp`, `path-corner`, `path-crossing`, `path-end`, `path-intersectionA`, `path-intersectionB`, `path-intersectionC`, `path-intersectionD`
- `path-intersectionE`, `path-intersectionF`, `path-intersectionG`, `path-intersectionH`, `path-square-end`, `path-square`, `path-start`, `path-straight`
- `river-corner-sharp`, `river-corner`, `river-crossing`, `river-end`, `river-intersectionA`, `river-intersectionB`, `river-intersectionC`, `river-intersectionD`
- `river-intersectionE`, `river-intersectionF`, `river-intersectionG`, `river-intersectionH`, `river-start`, `river-straight`, `sand-desert`, `sand-rocks`
- `sand`, `stone-hill`, `stone-mountain`, `stone-rocks`, `stone`, `unit-house`, `unit-mansion`, `unit-mill`
- `unit-ship-large`, `unit-ship`, `unit-tower`, `unit-tree`, `unit-wall-tower`, `water-island`, `water-rocks`, `water`

### kenney_mini-arena — 22 modèles

- `banner`, `block`, `border-corner`, `border-straight`, `bricks`, `character-soldier`, `column-damaged`, `column`
- `floor-detail`, `floor`, `stairs-corner-inner`, `stairs-corner`, `stairs`, `statue`, `tree`, `trophy`
- `wall-corner`, `wall-gate`, `wall`, `weapon-rack`, `weapon-spear`, `weapon-sword`

### kenney_mini-forest_1.0 — 22 modèles

- `bridge`, `building-platform`, `building-roof`, `building-structure`, `character-archer`, `fence`, `flag`, `ladder`
- `patch-dirt`, `patch-grass`, `plant`, `platform`, `rocks-high`, `rocks-low`, `rocks-ramp`, `stones`
- `target`, `tent`, `tree-high`, `tree`, `weapon-arrow`, `weapon-bow`

### kenney_skyboxes — 5 textures, 0 modèle

- `skybox-alien.png`, `skybox-day.png`, `skybox-morning.png`, `skybox-night.png`, `skybox-space.png`

### kenney_survival-kit — 80 modèles

- `barrel-open`, `barrel`, `bedroll-frame`, `bedroll-packed`, `bedroll`, `bottle-large`, `bottle`, `box-large-open`
- `box-large`, `box-open`, `box`, `bucket`, `campfire-fishing-stand`, `campfire-pit`, `campfire-stand`, `chest`
- `fence-doorway`, `fence-fortified`, `fence`, `fish-large`, `fish`, `floor-hole`, `floor-old`, `floor`
- `grass-large`, `grass`, `metal-panel-narrow`, `metal-panel-screws-half`, `metal-panel-screws-narrow`, `metal-panel-screws`, `metal-panel`, `patch-grass-large`
- `patch-grass`, `resource-planks`, `resource-stone-large`, `resource-stone`, `resource-wood`, `rock-a`, `rock-b`, `rock-c`
- `rock-flat-grass`, `rock-flat`, `rock-sand-a`, `rock-sand-b`, `rock-sand-c`, `signpost-single`, `signpost`, `structure-canvas`
- `structure-floor`, `structure-metal-doorway`, `structure-metal-floor`, `structure-metal-roof`, `structure-metal-wall`, `structure-metal`, `structure-roof`, `structure`
- `tent-canvas-half`, `tent-canvas`, `tent`, `tool-axe-upgraded`, `tool-axe`, `tool-hammer-upgraded`, `tool-hammer`, `tool-hoe-upgraded`
- `tool-hoe`, `tool-pickaxe-upgraded`, `tool-pickaxe`, `tool-shovel-upgraded`, `tool-shovel`, `tree-autumn-tall`, `tree-autumn-trunk`, `tree-autumn`
- `tree-log-small`, `tree-log`, `tree-tall`, `tree-trunk`, `tree`, `workbench-anvil`, `workbench-grind`, `workbench`

### kenney_toy-car-kit — 157 modèles

- `gate-finish`, `gate`, `item-banana`, `item-box`, `item-coin-bronze`, `item-coin-gold`, `item-coin-silver`, `item-cone`
- `smoke`, `supports-clamp`, `supports-diagonal`, `supports-wide-clamp`, `supports-wide-diagonal`, `supports-wide`, `supports`, `track-narrow-cap-back`
- `track-narrow-cap-front`, `track-narrow-corner-large-ramp`, `track-narrow-corner-large`, `track-narrow-corner-small-ramp`, `track-narrow-corner-small`, `track-narrow-curve`, `track-narrow-looping`, `track-narrow-straight-bend-large`
- `track-narrow-straight-bend`, `track-narrow-straight-bump-down`, `track-narrow-straight-bump-up`, `track-narrow-straight-hill-beginning`, `track-narrow-straight-hill-complete-half`, `track-narrow-straight-hill-complete`, `track-narrow-straight-hill-end`, `track-narrow-straight-skew-left-side`
- `track-narrow-straight-skew-left`, `track-narrow-straight-skew-right-side`, `track-narrow-straight-skew-right`, `track-narrow-straight`, `track-narrow`, `track-road-narrow-cap-back`, `track-road-narrow-cap-front`, `track-road-narrow-corner-large-ramp`
- `track-road-narrow-corner-large`, `track-road-narrow-corner-small-ramp`, `track-road-narrow-corner-small`, `track-road-narrow-curve`, `track-road-narrow-looping`, `track-road-narrow-straight-bend-large`, `track-road-narrow-straight-bend`, `track-road-narrow-straight-bump-down`
- `track-road-narrow-straight-bump-up`, `track-road-narrow-straight-hill-beginning`, `track-road-narrow-straight-hill-complete-half`, `track-road-narrow-straight-hill-complete`, `track-road-narrow-straight-hill-end`, `track-road-narrow-straight-skew-left-side`, `track-road-narrow-straight-skew-left`, `track-road-narrow-straight-skew-right-side`
- `track-road-narrow-straight-skew-right`, `track-road-narrow-straight`, `track-road-narrow`, `track-road-wide-cap-back`, `track-road-wide-cap-front`, `track-road-wide-corner-large-ramp`, `track-road-wide-corner-large`, `track-road-wide-corner-small-ramp`
- `track-road-wide-corner-small`, `track-road-wide-curve`, `track-road-wide-straight-bend-large`, `track-road-wide-straight-bend`, `track-road-wide-straight-bump-down`, `track-road-wide-straight-bump-up`, `track-road-wide-straight-hill-beginning`, `track-road-wide-straight-hill-complete-half`
- `track-road-wide-straight-hill-complete`, `track-road-wide-straight-hill-end`, `track-road-wide-straight-skew-left-side`, `track-road-wide-straight-skew-left`, `track-road-wide-straight-skew-right-side`, `track-road-wide-straight-skew-right`, `track-road-wide-straight`, `track-road-wide`
- `track-striped-narrow-cap-back`, `track-striped-narrow-cap-front`, `track-striped-narrow-corner-large-ramp`, `track-striped-narrow-corner-large`, `track-striped-narrow-corner-small-ramp`, `track-striped-narrow-corner-small`, `track-striped-narrow-curve`, `track-striped-narrow-looping`
- `track-striped-narrow-straight-bend-large`, `track-striped-narrow-straight-bend`, `track-striped-narrow-straight-bump-down`, `track-striped-narrow-straight-bump-up`, `track-striped-narrow-straight-hill-beginning`, `track-striped-narrow-straight-hill-complete-half`, `track-striped-narrow-straight-hill-complete`, `track-striped-narrow-straight-hill-end`
- `track-striped-narrow-straight-skew-left-side`, `track-striped-narrow-straight-skew-left`, `track-striped-narrow-straight-skew-right-side`, `track-striped-narrow-straight-skew-right`, `track-striped-narrow-straight`, `track-striped-narrow`, `track-striped-wide-cap-back`, `track-striped-wide-cap-front`
- `track-striped-wide-corner-large-ramp`, `track-striped-wide-corner-large`, `track-striped-wide-corner-small-ramp`, `track-striped-wide-corner-small`, `track-striped-wide-curve`, `track-striped-wide-straight-bend-large`, `track-striped-wide-straight-bend`, `track-striped-wide-straight-bump-down`
- `track-striped-wide-straight-bump-up`, `track-striped-wide-straight-hill-beginning`, `track-striped-wide-straight-hill-complete-half`, `track-striped-wide-straight-hill-complete`, `track-striped-wide-straight-hill-end`, `track-striped-wide-straight-skew-left-side`, `track-striped-wide-straight-skew-left`, `track-striped-wide-straight-skew-right-side`
- `track-striped-wide-straight-skew-right`, `track-striped-wide-straight`, `track-striped-wide`, `track-wide-cap-back`, `track-wide-cap-front`, `track-wide-corner-large-ramp`, `track-wide-corner-large`, `track-wide-corner-small-ramp`
- `track-wide-corner-small`, `track-wide-curve`, `track-wide-straight-bend-large`, `track-wide-straight-bend`, `track-wide-straight-bump-down`, `track-wide-straight-bump-up`, `track-wide-straight-hill-beginning`, `track-wide-straight-hill-complete-half`
- `track-wide-straight-hill-complete`, `track-wide-straight-hill-end`, `track-wide-straight-skew-left-side`, `track-wide-straight-skew-left`, `track-wide-straight-skew-right-side`, `track-wide-straight-skew-right`, `track-wide-straight`, `track-wide`
- `tree-pine`, `tree`, `vehicle-drag-racer`, `vehicle-monster-truck`, `vehicle-racer-low`, `vehicle-racer`, `vehicle-speedster`, `vehicle-suv`
- `vehicle-truck`, `vehicle-vintage-racer`, `wheel-large`, `wheel-medium`, `wheel-small`

### kenney_watercraft-pack — 46 modèles

- `arrow-standing`, `arrow`, `boat-fan`, `boat-fishing-small`, `boat-house-a`, `boat-house-b`, `boat-house-c`, `boat-house-d`
- `boat-row-large`, `boat-row-small`, `boat-sail-a`, `boat-sail-b`, `boat-speed-a`, `boat-speed-b`, `boat-speed-c`, `boat-speed-d`
- `boat-speed-e`, `boat-speed-f`, `boat-speed-g`, `boat-speed-h`, `boat-speed-i`, `boat-speed-j`, `boat-tow-a`, `boat-tow-b`
- `boat-tug-a`, `boat-tug-b`, `boat-tug-c`, `buoy-flag`, `buoy`, `cargo-container-a`, `cargo-container-b`, `cargo-container-c`
- `cargo-pile-a`, `cargo-pile-b`, `gate-finish`, `gate`, `ramp-wide`, `ramp`, `ship-cargo-a`, `ship-cargo-b`
- `ship-cargo-c`, `ship-large`, `ship-ocean-liner-small`, `ship-ocean-liner`, `ship-small-ghost`, `ship-small`

Le modèle « water waves » est de UJWAL, sous licence CC BY 4.0 selon les métadonnées du GLB fourni. Son animation a été adaptée au relief de la mer du niveau 2. Les métadonnées et la provenance sont conservées dans `Assets/Art/Level2/Terrain/WaterWaves/WaterWavesSource.json`. Source du modèle [water waves sur Sketchfab](https://sketchfab.com/3d-models/water-waves-142c2596d1944cd0bf4fc1f490c4c248). [Licence CC BY 4.0](http://creativecommons.org/licenses/by/4.0/).
