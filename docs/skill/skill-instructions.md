# Instructions opérationnelles détaillées

## 1. Mission
Tu modélises ou mets à jour un module métier CNSS sous forme DDD + Clean Architecture en restant aligné sur le style observé dans le repo d'exemple.

## 2. Priorités
1. préserver la continuité fonctionnelle du système cible
2. respecter le modèle validé existant
3. ancrer les propositions dans les sources
4. documenter séparément les hypothèses et ambiguïtés

## 3. Ressources à lire avant toute réponse
- `docs/conventions/DDD_CLEAN_CONVENTIONS.md`
- `docs/conventions/MODULE_REVIEW_CHECKLIST.md`
- `docs/agent/procedure-to-model-mapping-guide.md`
- `docs/agent/modeling-output-schema.json`
- `docs/templates/`
- modèle `current/` du module ciblé
- sources du module : manuels, lois, références, glossaire, ADR

## 4. Protocole de travail

### Étape 1. Identifier le mode de travail
Déterminer si la demande concerne :
- une création initiale
- une mise à jour incrémentale
- une extraction partielle de use cases
- une génération de Mermaid
- une analyse de conflit avec modèle existant

### Étape 2. Lire l'existant
Toujours lire en premier :
- le modèle validé courant
- les revues récentes
- les conventions officielles

Si aucun modèle n'existe :
- le signaler explicitement
- traiter la sortie comme une proposition initiale

### Étape 3. Analyser les sources
Extraire :
- acteurs
- tâches
- documents métier
- états métier
- règles métier
- dépendances externes
- termes du langage ubiquitaire

### Étape 4. Mapper vers le modèle
Produire :
- use cases
- entities
- value objects
- aggregates
- domain services
- repositories
- domain events
- structure Clean Architecture

### Étape 5. Comparer avec l'existant
Pour chaque élément nouveau ou modifié :
- vérifier s'il existe déjà
- détecter les collisions de vocabulaire
- détecter les conflits de responsabilité
- détecter les changements de frontière d'agrégat

### Étape 6. Produire la sortie
Toujours fournir :
- synthèse
- éléments observés
- hypothèses
- questions ouvertes
- conflits
- proposition structurée

## 5. Règles de modélisation
- considérer une tâche procédurale comme un use case candidat
- utiliser le vocabulaire métier des sources avant d'inventer des labels techniques
- n'introduire un value object que s'il encapsule un sens métier réel
- utiliser `Cnss.Shared.Domain.Abstractions.ValueObject` comme base officielle des value objects futurs
- n'introduire un domain service que si la logique n'appartient clairement à aucun agrégat
- générer les identifiants métier via un service de domaine
- laisser les identifiants techniques non métier à la base de données lorsque le module le permet
- privilégier une frontière d'agrégat compacte
- conserver `Aggregats` et `ValueObject` comme conventions de structure
- documenter la reconstruction des agrégats lorsqu'elle compte
- ne rendre la factory d'agrégat obligatoire que lorsqu'une contrainte conceptuelle le justifie
- proposer une mécanique `Commit()` ou `CommitAsync(bool flush = true)` dans les repositories du projet métier cible

## 6. Alignement avec le repo d'exemple
- un use case se projette en dossier `Request/Response/Handler/Validator`
- les contrats partagés inter-modules peuvent vivre dans une zone partagée analogue à `Shared.Application`
- les domain events sont des faits métier au passé
- la persistance est une projection d'infrastructure, pas la source du modèle métier
- les variantes historiques du repo ne doivent pas empêcher l'application des décisions de socle déjà actées

## 7. Gestion des hypothèses
Lorsqu'une règle n'est pas explicitement démontrée :
- la qualifier d'hypothèse
- expliquer sur quelle source partielle elle repose
- dire quel arbitrage ou document permettrait de la confirmer

## 8. Gestion des conflits
Si une proposition casse un modèle existant :
- le signaler avant toute recommandation
- décrire la rupture
- indiquer son impact sur la continuité fonctionnelle
- proposer une résolution prudente

## 9. Sortie minimale attendue
- `module_name`
- finalité du module
- bounded context
- acteurs
- langage ubiquitaire
- use cases
- modèle de domaine
- règles métier
- dépendances externes
- hypothèses
- questions ouvertes
- conflits
- structure Clean Architecture
- Mermaid

## 10. Ce qu'il ne faut pas faire
- produire une réponse "DDD universelle" sans ancrage CNSS
- renommer arbitrairement le vocabulaire existant
- masquer les ambiguïtés
- affirmer une règle juridique non visible dans les sources
- supprimer un concept du modèle existant sans justification
