# Ponyo — Niveau 2 : De la mer au village submergé

> **Document de référence pour un agent Codex travaillant sur Unity.**  
> **Statut :** spécification de game design et de level design (non implémentée).  
> **Durée visée :** **1 min 45 s (105 secondes)** pour une traversée réussie, hors morts et reprises.  
> **Périmètre :** **niveau 2 uniquement**. Le niveau 1 aquatique est réalisé par une autre partie de l'équipe et ne doit pas être modifié.

## 1. Vision et intention

Ponyo quitte l'océan, grimpe les marches qui mènent à la terre ferme et commence à courir sur la côte. Elle traverse une végétation ensoleillée, longe une falaise, puis une tempête s'installe et l'amène sur une route côtière où surgit une voiture aux pleins phares. La pluie fait monter l'eau, le village est submergé et Ponyo doit bondir entre des objets flottants et des vagues-poissons pour rejoindre une colline en hauteur, zone sûre et fin du niveau.

- **Genre :** platformer 2D à défilement horizontal, progression vers la droite.
- **Références :** *Ponyo sur la falaise* pour l'univers, *Super Mario Bros.* pour le terrain, les couches visuelles et la lisibilité des obstacles.
- **Ambiance :** lumineuse et rassurante au départ, de plus en plus sombre, venteuse et pluvieuse, puis spectaculaire et urgente dans la dernière scène.
- **Pas de Fujimoto, pas de boss, pas de combat.** Les dangers viennent des éléments naturels, des objets, des bulles et du trafic.
- **Priorité de réalisation :** fluidité des mouvements, obstacles lisibles, transition météo convaincante, montée des eaux et arrivée à la colline.

## 2. Organisation générale du niveau

Les quatre « scènes » ci-dessous désignent **quatre sections successives d'un même niveau**. Sauf contrainte technique existante, privilégier **une seule scène Unity** avec quatre segments contigus, plutôt que quatre changements de scène avec chargement.

| Section | Temps cible indicatif | Environnement | Mécanique principale |
|---|---:|---|---|
| **1 — Sortie de l'eau** | 0–20 s | Escalier côtier et végétation | Tutoriel de sauts, premières bulles et vague-poisson |
| **2 — Falaise végétalisée** | 20–45 s | Falaise, terre et herbe, mer en contrebas | Obstacles terrestres et rafales étourdissantes |
| **3 — Route sous l'orage** | 45–70 s | Route côtière, maisons au loin, ciel qui s'assombrit | Tempête, voiture aux pleins phares, seconde vague-poisson |
| **4 — Village submergé** | 70–105 s | Village inondé, plateformes flottantes, colline | Montée des eaux et succession de plateformes |

> Ces durées sont des **cibles de blockout**, à rééquilibrer après tests avec la vitesse de déplacement effective. Ne pas minuter chaque saut de façon rigide si cela détériore la jouabilité.

### Progression visuelle

```text
Océan (niveau 1, hors périmètre)
    ↓
[1] Escalier → bord de mer fleuri / soleil
    ↓
[2] Chemin en terre → falaise et clôtures / vent
    ↓
[3] Route goudronnée → pluie + ciel sombre / voiture
    ↓
[4] Village noyé → objets flottants + eau montante / urgence
    ↓
Colline surélevée → zone sûre / FIN
```

## 3. Direction artistique commune

### Caméra et profondeur des décors

- Caméra **latérale 2D** suivant Ponyo ; le parcours défile de gauche à droite.
- Reprendre la sensation des niveaux Mario avec un **arrière-plan à deux profondeurs** :
  1. **Fond lointain (parallaxe lente)** : ciel, horizon, mer lointaine, silhouettes d'îles.
  2. **Fond intermédiaire (parallaxe plus marquée)** : îles détaillées, reliefs, végétation, maisons/éléments du village selon la section.
- **Plan jouable (premier plan)** : sol, plateformes, végétation décorative proche, obstacles, Ponyo et objets interactifs.
- Les îles, buissons, maisons, arbres, fleurs et autres éléments doivent être **des assets séparés positionnables dans Unity**, pas un décor unique totalement aplati.
- Les obstacles doivent rester bien distinguables du décor : silhouettes nettes, contrastes suffisants, collisions intuitives.
- Le fond marin et les îles doivent accompagner la progression : d'abord lumineux et turquoise, puis assombris par la pluie et les nuages.

### Évolution météo et lumière

1. **Scène 1 :** ciel clair, mer lumineuse, végétation colorée.
2. **Scène 2 :** toujours lumineux, vent plus présent, quelques nuages possibles.
3. **Scène 3 :** **pluie légère au début**, puis augmentation progressive ; ciel et mer deviennent nettement plus sombres, sans changement brutal. À la fin, pluie abondante, atmosphère de tempête.
4. **Scène 4 :** pluie forte et ambiance orageuse ; eau agitée et niveau de l'eau qui monte réellement pendant le gameplay.

Prévoir des réglages modulables (intensité des particules, teinte du ciel, teinte de la mer, vitesse de montée de l'eau) pour pouvoir ajuster le rendu dans l'Inspector.

## 4. Conception détaillée des quatre scènes

### Scène 1 — Sortie de l'eau et premiers pas (0–20 s)

**Décor / composition**

- Début du niveau : **escalier ou marches côtières qui sortent directement de l'eau**, avec végétation sur les côtés.
- Ponyo monte sur la terre ferme, puis court dans une zone bordée d'herbe et de plantes.
- Fond lointain : mer calme et horizon ; **îles visibles au large** sous forme d'assets séparés.
- Fond intermédiaire : petites îles, roches et végétation côtière, pour donner de la profondeur.
- Assets décoratifs à disperser sur le sol jouable : **buissons, fleurs, touffes d'herbe, arbustes, petites pierres**.

**Obstacles et interactions**

| Élément | Comportement attendu |
|---|---|
| Rochers | Obstacles fixes à franchir par un saut. |
| Coquillages | Petits obstacles bas, visuellement distincts des simples décorations. |
| Bulles de capture | Bulles flottantes ; **si Ponyo les touche, elle est capturée**. Prévoir un retour au checkpoint ou une pénalité paramétrable (voir §7). |
| Flaques profondes | Zones au sol qui **ralentissent Ponyo temporairement** (pas de mort instantanée). |
| Petite vague-poisson | Vague en forme de poisson, mobile et animée ; **Ponyo peut rebondir dessus** pour franchir un obstacle ou rejoindre une plateforme. |

**Intentions de level design**

- Commencer par des obstacles simples et espacés pour introduire le saut.
- Présenter une bulle seule, facilement identifiable, avant toute combinaison difficile.
- Terminer sur une petite vague-poisson qui montre au joueur qu'une vague peut être une **aide** et pas seulement une menace.
- Aucun passage difficile nécessitant plusieurs mécaniques nouvelles simultanément.

### Scène 2 — La falaise et le chemin en terre (20–45 s)

**Décor / composition**

- Ponyo court sur un **chemin de terre surmonté d'herbe**, au bord d'une falaise.
- **Barrières en bois** et végétation le long du tracé ; quelques portions de clôtures à l'arrière-plan.
- **Poteau électrique / antenne électrique**, câbles et éléments de route côtière en arrière-plan.
- Mer et îles toujours visibles au loin, avec végétation sur les îles.
- Coupe visuelle « Mario » : le sol du chemin est clairement découpé, et l'on peut voir **une couche d'eau en dessous du terrain / de la falaise**. Cette eau est décorative ici, sauf si une zone est explicitement jouable.

**Obstacles et interactions**

| Élément | Comportement attendu |
|---|---|
| Barrière en bois | Segment d'obstacle à sauter (à distinguer des clôtures purement décoratives). |
| Ronces | Obstacle piquant au sol ; contact dangereux. |
| Bulles de capture | Réutilisation de l'ennemi de la scène 1, avec placement plus exigeant. |
| Rafale de vent | Rafale latérale qui surgit de la droite ; avertissement préalable, puis **étourdissement de Ponyo** si elle est touchée. |

**Rafale de vent — interaction distinctive**

1. Une **icône d'attention à droite de l'écran** prévient qu'une rafale arrive.
2. La rafale traverse une portion du parcours, avec effet visible de vent/feuilles.
3. Si Ponyo la touche, elle **tourne sur elle-même** et semble désorientée pendant **environ 1 seconde**.
4. Contrôle et vitesse sont temporairement altérés pendant cet étourdissement ; rétablissement automatique ensuite.
5. Les obstacles placés juste après doivent tenir compte de cette perte de contrôle pour ne pas provoquer une mort injuste.

**Intentions de level design**

- Introduire une première rafale dans une zone sécurisée, pour enseigner son effet.
- Ensuite seulement, combiner vent et obstacle terrestre.
- Utiliser les clôtures, le relief et les poteaux pour souligner visuellement la proximité du village.

### Scène 3 — Route côtière et arrivée de la tempête (45–70 s)

**Décor / composition**

- Transition progressive : le **sol jouable devient une route goudronnée**.
- Toujours sur le bord de la falaise, mais l'environnement s'urbanise progressivement.
- **Maisons visibles en arrière-plan** ; plus nombreuses à l'approche du village.
- Mer plus sombre, ciel qui se couvre ; pluie passant **de légère à abondante** pendant cette scène.
- Quelques clôtures, lampadaires et éléments routiers abîmés par le mauvais temps.

**Obstacles et interactions**

| Élément | Comportement attendu |
|---|---|
| Barrière renversée | Obstacle fixe horizontal ou oblique sur la route. |
| Lampadaire renversé | Obstacle imposant un saut et offrant une silhouette visuelle claire. |
| Voiture rapide aux pleins phares | Arrive de la droite, **fortement annoncée par ses phares**, puis traverse la route à grande vitesse. À esquiver, pas à combattre. |
| Rafales de vent | Même mécanique que scène 2, utilisée dans une situation plus intense. |
| Seconde vague-poisson | Réapparition de la plateforme / vague mobile, avec timing plus exigeant. |

**Voiture aux pleins phares — moment marquant**

1. Deux faisceaux lumineux apparaissent **depuis la droite**, avant que la voiture soit dangereuse.
2. Ajout possible d'un bruit de moteur / klaxon pour compléter l'alerte visuelle.
3. Une **voiture surgit rapidement de la droite vers la gauche** sur la route.
4. Ponyo doit l'esquiver en sautant ou en empruntant une petite zone surélevée prévue dans le parcours.
5. **Une seule voiture scriptée suffit** ; elle constitue un temps fort du niveau, pas un système de circulation complet.
6. Les pleins phares doivent rester lisibles **sans éblouir réellement le joueur**.

**Intentions de level design**

- Faire coïncider l'arrivée de la voiture avec le crescendo de pluie et de vent.
- Laisser un délai d'avertissement réglable et une issue toujours possible.
- La seconde vague-poisson prépare le joueur à s'en servir sous pression dans la scène 4.

### Scène 4 — Village submergé et montée des eaux (70–105 s)

**Décor / composition**

- Entrée dans le **village en cours d'inondation** : maisons, clôtures, éléments urbains et objets partiellement engloutis.
- Les pluies sont désormais fortes et **le niveau de l'eau monte visiblement pendant le jeu**.
- Les plateformes flottent : caisses, barques, éléments emportés par l'eau et vagues-poissons.
- Le parcours devient plus vertical ; Ponyo doit progressivement atteindre des plateformes de plus en plus hautes.
- À la fin, une **colline surélevée** sort des eaux : arrivée de Ponyo sur ce terrain sec = **fin du niveau**.

**Obstacles et interactions**

| Élément | Comportement attendu |
|---|---|
| Eau montante dangereuse | **Ne pas tomber dans l'eau** : dans ce niveau, elle est considérée comme contaminée / dangereuse. Le contact déclenche un échec ou un retour au checkpoint. |
| Plateformes flottantes | Montent avec l'eau, éventuellement oscillent légèrement ; Ponyo peut y atterrir. |
| Barque avec crabe | Plateforme flottante occupée par un **crabe à éviter**. Le joueur doit gérer le saut d'arrivée ou l'esquive sur la barque. |
| Caisse fragile | Plateforme utilisable, mais **se brise ~1 seconde après le premier contact de Ponyo** : impossible d'attendre dessus. |
| Vagues-poissons | Plateformes mobiles / rebondissantes essentielles à la progression. |
| Jet d'eau | Jet ascendant ou latéral annoncé, qui projette ou déstabilise Ponyo ; comportement précis à choisir lors du prototype. |
| Montée rapide des eaux | Pression principale de la scène : le joueur doit rester en mouvement et monter vers la colline. |

**Montée des eaux — logique de gameplay**

- La montée commence au début de la scène 4 et **s'accélère par étapes** vers la fin.
- Les plateformes flottantes suivent le niveau d'eau, selon des paramètres de flottabilité simplifiés ou un mouvement scripté.
- Prévoir des **zones sûres temporaires** pour rendre la séquence difficile mais juste.
- La progression peut être scriptée par points de passage : activation d'un nouvel obstacle, hausse de l'eau, nouvelle plateforme accessible, etc.
- Le niveau se termine **quand Ponyo atteint le sommet de la colline**, pas en combattant un adversaire.

**Intentions de level design**

- Début : comprendre le déplacement des plateformes avec une première caisse et une barque.
- Milieu : combiner une caisse cassable, une vague-poisson et l'eau qui monte.
- Fin : courte montée finale rapide vers la colline, sans ajout d'une mécanique totalement nouvelle.

## 5. Contrôles et déplacement (base à valider)

Conserver l'esprit d'un runner 2D accessible, déjà envisagé pour ce niveau :

- Ponyo **avance automatiquement vers la droite** ; vitesse paramétrable.
- **Pression courte :** petit saut.
- **Pression maintenue :** saut plus haut / plus long.
- **Option à valider en prototype :** deuxième pression en l'air après relâchement = petit plané temporaire ; ne pas l'imposer si cela complique inutilement les 105 secondes.
- **Rebond sur vague-poisson :** déclenché à l'atterrissage sur sa surface rebondissante.
- Les états « ralenti dans une flaque », « capturé par une bulle » et « étourdi par le vent » doivent être clairement différenciés.
- Prévoir **coyote time** et **jump buffer** paramétrables pour rendre les sauts agréables.
- Les collisions doivent être cohérentes avec les silhouettes ; les obstacles mortels doivent être annoncés.

## 6. Découpage technique conseillé sous Unity

> Il s'agit d'une **proposition d'architecture** pour l'agent, à adapter à la structure réelle du dépôt. **Inspecter les scripts et packages existants avant de créer des doublons.**

### Systèmes

| Système / composant suggéré | Responsabilité |
|---|---|
| `PonyoMovement2D` | Course auto, saut variable, collisions et rebonds. |
| `PonyoStatusEffects` | Ralentissement, stun / rotation de confusion, éventuelle capture. |
| `CameraFollow2D` / Cinemachine | Suivi horizontal, recadrage vertical en scène 4. |
| `ParallaxLayer` | Deux profondeurs de fond à vitesses différentes. |
| `BubbleTrap` | Détection de capture de Ponyo par les bulles. |
| `FishWavePlatform` | Déplacement et rebond sur vague-poisson. |
| `WindGust` + `HazardWarningUI` | Déclenchement de rafale et icône d'alerte venant de la droite. |
| `CarHazard` | Apparition scriptée de la voiture, préavis par phares, déplacement et collision. |
| `WeatherDirector` | Transition de pluie / couleurs / ambiance entre scènes 2, 3 et 4. |
| `FloodController` | Hauteur et vitesse de montée de l'eau ; zone mortelle. |
| `FloatingPlatform` | Plateforme suivant la hauteur de l'eau / oscillation légère. |
| `BreakableCrate` | Compte à rebours de 1 s au premier atterrissage, puis destruction. |
| `WaterJetHazard` | Jet d'eau déclenché et annoncé. |
| `CheckpointManager` | Reprise après collision mortelle, état du niveau restauré. |
| `LevelEndTrigger` | Détection de la colline et séquence de fin. |

### Organisation recommandée

```text
Assets/
  Scenes/
    Level2_Ponyo.unity
  Scripts/Level2/
    Player/
    Obstacles/
    Environment/
    LevelFlow/
  Prefabs/Level2/
    Obstacles/
    Platforms/
    Decorations/
  Art/Level2/
    Characters/
    Backgrounds/
    Terrain/
    Props/
    Effects/
  Audio/Level2/
```

Utiliser **Prefabs** pour les obstacles répétables, **SpriteShape ou Tilemap** pour les sols, des **sorting layers** explicites pour les plans, et des champs `[SerializeField]` / ScriptableObjects lorsque pertinent pour régler rapidement les timings. Éviter de modifier les scènes et scripts du niveau 1.

## 7. Règles de collision, échec et checkpoints

### Confirmé par le concept

- Tomber dans l'eau de la scène 4 = **échec** (eau dangereuse / contaminée).
- Toucher une bulle = **Ponyo est capturée**.
- Toucher une rafale = **étourdissement temporaire d'environ 1 s** avec rotation visuelle.
- Atterrir sur une caisse fragile = **elle casse au bout d'environ 1 s**.
- Atteindre la colline = **victoire / fin du niveau**.

### Proposition à valider avec l'équipe

- Une bulle capturant Ponyo entraîne après une courte animation le **retour au checkpoint**.
- Voiture, ronces et principaux dangers déclenchent un **retour au checkpoint**.
- Prévoir des checkpoints à la fin des scènes 1, 2 et 3, avec **restauration de la météo et de la hauteur de l'eau** correspondantes.
- L'étourdissement de vent ne doit pas provoquer automatiquement une mort inévitable : conserver un délai et un placement compatibles avec la reprise de contrôle.
- La durée de 105 secondes correspond au parcours sans échec ; les reprises peuvent allonger la durée totale.

## 8. Inventaire des assets nécessaires

### Personnage et animation

- Ponyo : idle / début de course, course, petit saut, grand saut, chute, atterrissage, rebond.
- Ponyo : animation **capturée dans une bulle**, animation **confuse qui tourne sur elle-même**, victoire sur la colline.
- Option : animation de plané si la mécanique est conservée.

### Fonds (séparés pour parallaxe)

- Ciel clair, ciel nuageux, ciel orageux (ou transitions de couleur pilotées par shader).
- Mer lointaine claire puis sombre, **îles au large** en plusieurs sprites, reliefs côtiers, végétation distante.
- Falaise, poteaux/câbles, maisons distantes, silhouettes du village.
- Éléments visuels de tempête : nuages, pluie légère / forte, brume / éclaboussures.

### Terrains et décors plaçables

- Escalier sortant de la mer, terre + herbe, chemin côtier, falaise en coupe, route goudronnée.
- Buissons, fleurs, touffes d'herbe, arbustes, petites pierres, végétation côtière.
- Barrières en bois (debout et renversées), ronces, poteau électrique, lampadaire droit / renversé.
- Maisons de village, colline de fin de niveau.

### Obstacles / plateformes / effets

- Rochers, coquillages, bulles de capture (sprite + animation), flaques profondes.
- Vague-poisson **petite** et **seconde variante**, avec animation de déplacement et rebond.
- Rafale de vent, feuilles soulevées, **icône d'alerte à droite de l'écran**, effet de stun.
- Voiture rapide, **faisceaux des pleins phares**, éventuelle poussière / éclaboussures.
- Eau montante, vague de surface, caisses flottantes, caisses cassables, barques, crabe, jets d'eau.
- Effets de pluie forte, éclaboussures, destruction des caisses, signal de checkpoint et fin.

### Sons indicatifs

- Ambiance mer / oiseaux ; pas de Ponyo ; petits sauts et rebonds.
- Bulle de capture ; vent qui s'annonce puis frappe ; petit étourdissement.
- Pluie légère puis forte ; moteur / klaxon ; bruit de vague-poisson.
- Bois qui craque et caisse qui se brise ; montée des eaux ; signal de fin.

> Les assets sont à **créer, dessiner, générer puis retoucher, ou obtenir avec une licence adaptée**. Ne pas supposer que des images extraites du film sont librement réutilisables ou redistribuables.

## 9. Ordre de développement recommandé pour Codex

**Étape A — Blockout jouable**

1. Inspecter le projet existant, sa version Unity, ses packages, conventions et scènes.
2. Créer **uniquement le niveau 2** avec 4 sections consécutives et des placeholders.
3. Implémenter le contrôleur de Ponyo et une caméra à défilement horizontal.
4. Disposer les terrains de base et valider la **traversée en environ 105 secondes**.

**Étape B — Obstacles et interactions**

5. Ajouter rochers, coquillages, flaques et bulles.
6. Créer le prefab de vague-poisson rebondissante.
7. Ajouter ronces, barrières, rafales avec icône d'alerte et stun.
8. Implémenter la voiture scriptée aux pleins phares.

**Étape C — Climax aquatique**

9. Implémenter la montée des eaux, les plateformes flottantes et le danger lié à l'eau.
10. Ajouter la barque avec crabe, la caisse cassable à 1 s et les jets d'eau.
11. Placer la colline et le trigger de fin ; intégrer les checkpoints.

**Étape D — Habillage et polish**

12. Poser les deux plans de parallaxe, îles, maisons et assets de végétation.
13. Créer la transition météo et colorimétrique, puis les effets sonores et animations.
14. Tester lisibilité, équité des pièges, performances et durée réelle du niveau.

## 10. Critères d'acceptation

- [ ] Le niveau 2 commence sur un **escalier sortant de l'océan** et se termine sur une **colline sèche**.
- [ ] Les quatre sections se suivent avec une transition fluide, dans l'ordre spécifié.
- [ ] La caméra montre **deux profondeurs de parallaxe** (mer/îles lointaines et éléments intermédiaires), plus le plan jouable.
- [ ] La scène 1 contient rochers, coquillages, bulles, flaques et une vague-poisson rebondissante.
- [ ] La scène 2 contient chemin de falaise en terre/herbe, eau visible sous le terrain, barrières, ronces et **vent qui étourdit ~1 s avec alerte à droite**.
- [ ] La scène 3 passe sur la **route**, affiche maisons et **pluie progressivement plus forte**, et comporte une **voiture rapide annoncée par ses pleins phares**.
- [ ] La scène 4 contient **eau montante dangereuse**, plateformes flottantes, barque + crabe, caisse détruite après ~1 s, vagues-poissons et jet d'eau.
- [ ] Les obstacles récurrents sont des Prefabs réutilisables et leurs paramètres majeurs sont réglables dans l'Inspector.
- [ ] Les collisions, animations de danger et avertissements sont lisibles avant de devenir punitifs.
- [ ] Les checkpoints (si retenus) restaurent un état cohérent des objets et de l'eau.
- [ ] Une traversée réussie vise **environ 1 min 45 s**, à vérifier par playtest.
- [ ] Aucun système Fujimoto, boss ou combat n'est introduit.
- [ ] Le **niveau 1** reste intact.

## 11. Consignes explicites pour l'agent

1. **Ne considère pas ce document comme du code déjà présent.** Inspecte d'abord le dépôt et les scènes Unity.
2. Respecte **les quatre sections et les éléments demandés**, sans ajouter d'ennemis majeurs ou de mécanique de combat.
3. **N'invente pas de sprites définitifs** : utilise des placeholders clairement identifiés si les assets manquent, et fournis leur liste.
4. Sépare **logique de gameplay**, **présentation visuelle** et **déclencheurs de progression**.
5. Implémente les risques (voiture, eau, ronces, bulles, rafales) de façon lisible et paramétrable ; privilégie un gameplay déterministe pour le runner.
6. Si les décisions ouvertes (mort sur bulle, checkpoints, jet d'eau, plané) affectent l'implémentation, applique les **propositions par défaut** indiquées dans ce fichier et signale-les dans le compte rendu.
7. Après chaque étape, indique les fichiers créés ou modifiés, les tests effectués et les assets encore manquants.

---

**Résumé du niveau :** Ponyo sort de l'océan, franchit les obstacles d'une côte fleurie, traverse une falaise venteuse, esquive une voiture sur une route gagnée par l'orage, puis saute de barque en caisse et de vague-poisson en vague-poisson pour échapper aux eaux qui submergent le village et atteindre une colline sûre.
