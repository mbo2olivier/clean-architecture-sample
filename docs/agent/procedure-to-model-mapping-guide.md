# Guide de dérivation procédure -> modèle DDD / Clean Architecture

## Objet
Ce guide explique comment transformer un manuel de procédure CNSS en modèle exploitable par un skill de modélisation, en restant compatible avec le style observé dans le repo d'exemple.

## 1. Point de départ
Toujours partir des sources dans cet ordre :
1. modèle validé existant du module
2. dernière version approuvée des manuels de procédure
3. textes légaux et réglementaires applicables
4. glossaire métier et décisions d'architecture

Règle :
- ne jamais proposer un nouveau modèle sans comparer avec la dernière version validée.

## 2. Dériver les acteurs
Chercher dans le manuel :
- qui initie l'action
- qui contrôle ou valide
- qui exécute matériellement la tâche
- qui reçoit le résultat

Indices linguistiques utiles :
- "l'agent"
- "le gestionnaire"
- "l'employeur"
- "l'assuré"
- "le service"
- "la caisse"

Sortie attendue :
- liste d'acteurs avec rôle métier précis
- distinction entre acteur humain, système interne et système externe

## 3. Dériver les tâches
Dans l'application cible, les fonctionnalités existantes sont décrites comme des tâches proches des use cases.

Méthode :
- isoler chaque verbe d'action métier stable
- regrouper les étapes qui poursuivent un même résultat métier
- séparer les vérifications préalables des résultats attendus

Exemples de marqueurs :
- "enregistrer"
- "valider"
- "affilier"
- "attacher"
- "publier"
- "calculer"
- "rejeter"

Sortie attendue :
- une tâche = un candidat principal de use case
- les sous-étapes deviennent le flux du use case

## 4. Dériver les use cases
Pour chaque tâche candidate :
- identifier l'acteur primaire
- identifier l'événement déclencheur
- décrire le résultat métier attendu
- extraire les données d'entrée minimales
- lister les dépendances externes ou inter-modules

Mapping conseillé vers le style du repo :
- un use case devient un dossier applicatif
- il contient `Request`, `Response`, `Handler`, `RequestValidator`

Indices de typage :
- si la tâche modifie l'état métier : `command`
- si la tâche lit sans modifier : `query`
- si le manuel mélange lecture et décision : `mixed` jusqu'à clarification

## 5. Dériver les règles métier
Repérer :
- les conditions obligatoires
- les seuils, bornes, délais
- les interdictions
- les cas d'exception
- les pièces justificatives imposées
- les règles de calcul

Formulation utile :
- "doit"
- "ne peut pas"
- "uniquement si"
- "au plus"
- "au moins"
- "dans un délai de"

Sortie attendue :
- règle formulée en phrase courte
- source précise
- statut : `observed`, `derived`, `hypothesis`

## 6. Dériver les documents métier
Chercher les objets documentaires qui traversent plusieurs étapes :
- formulaire
- décision
- déclaration
- dossier
- demande
- attestation
- ordre de paiement

Interprétation :
- si le document porte identité, cycle de vie et règles propres, c'est souvent une entité ou un agrégat
- si le document est une simple donnée descriptive stable, cela peut être un value object

## 7. Dériver les états métier
Repérer :
- états explicites : "brouillon", "validé", "rejeté", "liquidé", "publié"
- transitions implicites : "après contrôle", "après paiement", "après validation"

Usage :
- un état peut devenir un champ d'entité
- une transition importante peut justifier un domain event
- une contrainte d'état devient un invariant d'agrégat

## 8. Dériver les entities
Une entity est un bon candidat si l'objet :
- possède une identité métier propre
- survit à plusieurs opérations
- change au cours du temps
- doit être retrouvé indépendamment

Questions de test :
- a-t-il un identifiant métier ?
- peut-il être modifié sans changer de nature ?
- est-il référencé par d'autres concepts ?

## 9. Dériver les value objects
Un value object est un bon candidat si le concept :
- n'a pas d'identité propre
- est défini par ses attributs
- encapsule une validation ou un format métier

Exemples compatibles avec le repo :
- période
- identifiant typé
- paquet de données d'entrée
- sous-structure de calcul

Décision de socle :
- pour les futurs modules, la base officielle à utiliser est `Cnss.Shared.Domain.Abstractions.ValueObject`.

## 10. Dériver les aggregates
Former un agrégat autour d'un centre de cohérence transactionnelle.

Questions :
- quelles règles doivent être vraies ensemble immédiatement ?
- quel objet contrôle les transitions majeures ?
- quelles entités peuvent rester référencées seulement par identifiant ?

Alignement avec le repo :
- agrégat racine scellé
- méthodes métier explicites
- domain events émis par la racine
- reconstruction possible via `Restore(...)` si nécessaire

## 11. Dériver les domain services
Créer un domain service seulement si la logique :
- n'appartient clairement à aucune entité
- exprime une opération métier importante
- agit sur plusieurs objets ou encapsule une politique métier

Exemples issus du repo :
- génération d'identifiants
- assemblage structuré d'un agrégat

Décision de socle :
- la génération des identifiants métier doit relever d'un service de domaine dédié.
- les identifiants techniques non métier peuvent être laissés à la base de données.

## 12. Dériver les repositories
Créer un repository pour chaque agrégat qu'il faut :
- charger
- ajouter
- mettre à jour
- rechercher de manière métier

Convention cible issue du repo :
- interface dans `Domain/Repositories`
- implémentation dans `Infrastructure/Repositories`

Décision de socle :
- le repository du projet métier cible doit prévoir un `Commit()` ou `CommitAsync(bool flush = true)` pour contrôler la validation transactionnelle.
- le skill doit donc distinguer les opérations d'écriture métier des opérations de commit.

## 13. Dériver les domain events
Créer un domain event lorsqu'un fait métier accompli :
- intéresse un autre module
- marque une transition de cycle de vie
- doit être historisé ou publié

Forme cible observée :
- nom au passé métier
- payload métier utile
- publication technique possible via outbox

## 14. Dériver la structure Clean Architecture

### Domaine
Mettre dans le domaine :
- entities
- value objects
- agrégats
- domain services
- repositories interfaces
- domain events
- services de génération d'identifiants métier si le module manipule de tels identifiants

### Application
Mettre dans l'application :
- use cases
- validation de requêtes
- orchestration inter-modules
- mapping entre contrats et domaine

### Infrastructure
Mettre dans l'infrastructure :
- EF Core
- mapping relationnel
- repositories concrets
- outbox
- messaging
- providers techniques

### Entrypoints
Mettre dans l'entrypoint :
- API HTTP
- pages UI
- binding des entrées
- adaptation erreur/réponse

## 15. Gestion des conflits avec modèle existant
Avant toute proposition :
- comparer chaque nouvel élément au modèle validé courant
- lister les collisions de vocabulaire
- lister les doublons d'entités ou use cases
- signaler toute rupture de structure ou de sens

Sortie attendue :
- `conflicts_with_existing_model`
- `assumptions`
- `open_questions`

## 16. Règle opérationnelle pour le futur skill
Si la source procédurale est ambiguë :
- ne pas inventer silencieusement
- produire une hypothèse nommée
- rattacher l'hypothèse à sa source
- ouvrir une question explicite

Si le choix entre méthode statique, factory ou restauration explicite est en jeu :
- ne pas imposer de pattern unique
- choisir la forme la plus cohérente avec la contrainte conceptuelle observée
- justifier ce choix dans les notes de modélisation
