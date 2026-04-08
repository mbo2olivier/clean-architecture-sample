# Exemples d'utilisation

## 1. Génération initiale d'un module
```text
Construis la première modélisation du module cotisations à partir du manuel de procédure fourni et des extraits réglementaires joints. Utilise les conventions du repo, sépare observations, hypothèses et questions ouvertes, et produis aussi le JSON de sortie.
```

## 2. Mise à jour incrémentale d'un module existant
```text
Mets à jour le module pensions en partant de `modules/pensions/current/`. Intègre la nouvelle procédure de liquidation, compare avec le modèle courant et liste explicitement les conflits, impacts et hypothèses.
```

## 3. Génération Mermaid
```text
À partir du modèle actuel du module assujettissement et du nouveau draft, génère trois diagrammes Mermaid : flux acteur -> use case, carte des agrégats et cycle de vie métier principal.
```

## 4. Détection de conflit avec modèle existant
```text
Analyse ce manuel de procédure du front-office et compare-le avec `modules/front-office/current/`. Signale les divergences de vocabulaire, les use cases manquants et les changements de frontière d'agrégat.
```

## 5. Extraction de use cases depuis un manuel
```text
Lis ce manuel de procédure prestations familiales, extrais les tâches métier et propose leur mapping en use cases compatibles avec le style observé du repo d'exemple, sans encore figer tout le domaine.
```

