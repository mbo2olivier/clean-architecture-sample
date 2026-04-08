# CNSS DDD Clean Modeler

## Description
Compétence Codex spécialisée dans la création et la mise à jour incrémentale de modèles DDD + Clean Architecture pour des modules métier CNSS gouvernés par règles métier, manuels de procédure, textes légaux et modèles existants.

## Objectif
Produire un modèle exploitable, traçable et incrémental d'un module CNSS en s'appuyant d'abord sur la réalité documentaire et sur les conventions observées dans le repo d'exemple.

## Domaine d'application
- modules métier CNSS complexes
- modélisation de bounded contexts
- extraction de use cases depuis des tâches procédurales
- mise à jour contrôlée d'un modèle existant
- production de schémas Mermaid et de structures Clean Architecture

## Entrées attendues
- module ciblé
- manuel de procédure ou extrait
- textes légaux et réglementaires applicables
- modèle existant validé du module, s'il existe
- glossaire et décisions d'architecture disponibles
- demande utilisateur : création initiale ou mise à jour incrémentale

## Workflow détaillé
1. Lire les conventions, templates et protocoles du repo mémoire.
2. Lire le modèle existant validé du module avant toute proposition.
3. Lire les nouvelles sources documentaires.
4. Extraire acteurs, tâches, règles métier, documents, états et dépendances externes.
5. Mapper les tâches vers des use cases compatibles avec le style observé du repo d'exemple.
6. Dériver entities, value objects, aggregates, domain services, repositories et domain events.
7. Appliquer les décisions de socle sur `Aggregats`, `ValueObject`, génération des identifiants métier et commit transactionnel.
8. Produire la structure Clean Architecture suggérée.
9. Comparer explicitement avec le modèle existant.
10. Séparer `conventions observées`, `hypothèses` et `questions ouvertes`.
11. Générer les artefacts de sortie documentaires et structurés.

## Sorties attendues
- synthèse métier du module
- schéma JSON conforme à `docs/agent/modeling-output-schema.json`
- documentation de modèle
- liste des hypothèses
- liste des questions ouvertes
- conflits avec le modèle courant
- diagrammes Mermaid
- structure Clean Architecture suggérée

## Garde-fous
- ne jamais inventer silencieusement une règle métier absente des sources
- ne jamais remplacer un modèle existant sans signaler les écarts
- ne jamais traiter un brouillon comme une vérité officielle
- ne jamais construire une architecture "idéale" déconnectée du style observé
- marquer explicitement toute déduction non prouvée
- appliquer les décisions de socle même lorsqu'elles corrigent une variante historique du repo d'exemple

## Limites
- le skill ne valide pas juridiquement les textes
- le skill ne tranche pas seul les ambiguïtés métier majeures
- le skill ne remplace pas une revue métier ou architecture
- le skill ne doit pas déduire une transaction technique détaillée hors de la convention de commit retenue pour le projet métier

## Création initiale vs mise à jour incrémentale

### Création initiale
- partir d'un module vide ou quasi vide
- créer un premier modèle structuré
- signaler les zones faibles en hypothèses et questions ouvertes

### Mise à jour incrémentale
- partir impérativement de la dernière version validée
- calculer un delta explicite
- lister les conflits avec l'existant
- préserver autant que possible le vocabulaire et la continuité fonctionnelle
