# Structure cible de la mémoire CNSS

## Objectif
Cette structure cible définit comment organiser un repo mémoire CNSS destiné à la modélisation incrémentale des modules métier :
- administration
- référentiel
- front-office
- assujettissement
- cotisations
- prestations familiales
- pensions
- risques professionnels

Le repo mémoire n'est pas un repo d'exécution applicative. C'est un repo de vérité documentaire et de modélisation versionnée.

## 1. Structure recommandée du repo mémoire

```text
/
├── AGENTS.md
├── docs/
│   ├── conventions/
│   ├── agent/
│   ├── templates/
│   ├── cnss/
│   └── governance/
├── modules/
│   ├── administration/
│   ├── referentiel/
│   ├── front-office/
│   ├── assujettissement/
│   ├── cotisations/
│   ├── prestations-familiales/
│   ├── pensions/
│   └── risques-professionnels/
└── shared/
    ├── glossary/
    ├── legal/
    ├── procedures/
    ├── architecture-decisions/
    └── reference-models/
```

## 2. Rôle de chaque dossier

### `docs/conventions/`
Contient les conventions officielles du repo mémoire et celles dérivées du repo d'exemple.

### `docs/agent/`
Contient le schéma de sortie, les protocoles de travail du skill et les guides de mapping.

### `docs/templates/`
Contient les modèles minimaux réutilisables pour produire un module ou une mise à jour.

### `docs/cnss/`
Contient la mémoire transverse métier CNSS :
- principes métier globaux
- carte des modules
- règles de continuité fonctionnelle
- conventions de vocabulaire

### `docs/governance/`
Contient les règles de validation, revue, statut documentaire et processus d'acceptation.

### `modules/<module>/`
Contient toute la mémoire spécifique d'un module métier.

### `shared/glossary/`
Glossaire transverse, termes officiels et synonymes interdits ou tolérés.

### `shared/legal/`
Textes de loi, règlements, circulaires, références normatives indexées.

### `shared/procedures/`
Manuels de procédure source, organisés et versionnés.

### `shared/architecture-decisions/`
Décisions d'architecture applicables à plusieurs modules.

### `shared/reference-models/`
Modèles de référence inter-modules, patterns réutilisables et structures partagées.

## 3. Structure détaillée d'un module avant alimentation

```text
modules/<module>/
├── README.md
├── current/
│   └── README.md
├── drafts/
│   └── README.md
├── history/
│   └── README.md
├── reviews/
│   └── README.md
├── sources/
│   ├── procedures/
│   ├── legal/
│   └── references/
└── workspace/
    └── README.md
```

### Rôle avant alimentation
- `current/` : emplacement de la dernière version validée ; peut être vide au départ.
- `drafts/` : propositions non validées par le skill ou les analystes.
- `history/` : anciennes versions validées, archivées et datées.
- `reviews/` : comptes rendus de revue, arbitrages, demandes de correction.
- `sources/` : matière première documentaire du module.
- `workspace/` : espace de travail temporaire, non officiel.

## 4. Structure détaillée d'un module après alimentation par le skill

```text
modules/<module>/
├── README.md
├── current/
│   ├── module-context.md
│   ├── ubiquitous-language.md
│   ├── use-cases.md
│   ├── domain-model.md
│   ├── business-rules.md
│   ├── clean-architecture-mapping.md
│   ├── external-dependencies.md
│   ├── mermaid/
│   │   ├── context-flow.md
│   │   ├── aggregate-map.md
│   │   └── lifecycle.md
│   └── modeling-output.json
├── drafts/
│   └── YYYY-MM-DD_<change-slug>/
│       ├── proposed-module-context.md
│       ├── proposed-domain-model.md
│       ├── proposed-use-cases.md
│       ├── delta-vs-current.md
│       ├── assumptions.md
│       └── open-questions.md
├── history/
│   └── YYYY-MM-DD_vNN/
│       ├── module-context.md
│       ├── domain-model.md
│       ├── use-cases.md
│       └── modeling-output.json
├── reviews/
│   └── YYYY-MM-DD_<review-slug>.md
├── sources/
│   ├── procedures/
│   ├── legal/
│   └── references/
└── workspace/
    └── YYYY-MM-DD_<experiment-slug>.md
```

## 5. Conventions de nommage des fichiers

### Règles générales
- minuscules
- kebab-case
- dates au format `YYYY-MM-DD`
- versions au format `vNN`
- fichiers stables sans date dans `current/`
- fichiers datés dans `drafts/`, `history/`, `reviews/`, `workspace/`

### Exemples
- `domain-model.md`
- `business-rules.md`
- `2026-04-08_affiliation-initial-import/`
- `2026-04-08_v03/`
- `2026-04-08_review-gap-on-benefit-eligibility.md`

## 6. Distinction `current` / `drafts` / `history` / `reviews`

### `current`
Mémoire officielle active.

Contient :
- la version de référence sur laquelle les prochaines mises à jour doivent s'appuyer
- uniquement du contenu validé

### `drafts`
Propositions en cours de travail.

Contient :
- nouvelles modélisations
- propositions de delta
- hypothèses en cours de levée
- documents préparatoires d'une prochaine validation

### `history`
Archives officielles précédemment validées.

Contient :
- snapshots gelés
- anciennes versions de référence
- artefacts permettant d'expliquer l'évolution

### `reviews`
Journal de revue et d'arbitrage.

Contient :
- avis reviewers
- décisions
- demandes de correction
- conflits constatés avec l'existant

## 7. Mémoire officielle vs brouillons de travail

### Mémoire officielle
Relève de la mémoire officielle :
- tout ce qui est dans `current/`
- tout ce qui est dans `history/`
- les revues validées servant de trace d'arbitrage

### Brouillons de travail
Relève du travail non officiel :
- `drafts/`
- `workspace/`
- notes préparatoires non approuvées

Règle forte :
- le skill ne doit jamais remplacer silencieusement `current/` par un brouillon.

## 8. Structure recommandée du `README.md` module

```text
# <Module>
- objectif métier
- périmètre
- version courante
- sources principales
- statut du modèle
- liens vers current, drafts, reviews
```

## 9. Protocole de mise à jour incrémentale
1. Lire `current/`.
2. Lire les `reviews/` récentes.
3. Lire les nouvelles sources dans `sources/`.
4. Produire un draft daté avec delta explicite.
5. Ne promouvoir vers `current/` qu'après validation.
6. Archiver l'ancienne version validée dans `history/`.

## 10. Recommandation spécifique CNSS
Comme l'objectif est de préserver la continuité fonctionnelle et de limiter la rupture utilisateur :
- chaque draft doit expliciter le mapping entre fonctionnalités existantes, tâches actuelles et nouveaux use cases
- tout renommage d'un concept métier existant doit être justifié
- tout conflit entre texte légal, manuel et modèle courant doit être isolé dans `reviews/` et `open questions`

