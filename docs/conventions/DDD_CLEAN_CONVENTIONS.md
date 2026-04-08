# Conventions DDD + Clean Architecture observées dans le repo

## Objet du document
Ce document formalise le style réellement observé dans le dépôt de référence `Cnss-Clean-Architecture`.

Il distingue systématiquement :
- `Observé` : visible directement dans le code.
- `Variante` : plusieurs styles coexistent dans le repo.
- `Hypothèse` : proposition raisonnable non entièrement prouvée par le code.
- `Décision` : convention officialisée pour le futur socle, même si le repo d'exemple présente des variantes.

## 1. Découpage des couches

### 1.1 Structure globale observée
Chaque sous-domaine métier est porté par plusieurs projets .NET distincts :
- `*.Domain`
- `*.Application`
- `*.Infrastructure`
- un point d'entrée UI/API selon le module (`*.Portal`, `*.Api`)

Modules observés :
- `Affiliation`
- `Cotisation`
- `Shared`

### 1.2 Responsabilités observées
- `Domain` : agrégats, entités, value objects, événements de domaine, repositories interfaces, services/factories de domaine.
- `Application` : use cases orientés requêtes/commandes avec `Request`, `Response`, `Handler`, `Validator`.
- `Infrastructure` : EF Core, mappings, repositories concrets, outbox, configuration technique, hébergement de processors.
- `Api` ou `Portal` : adaptation HTTP/UI vers les use cases applicatifs via `IMediator`.
- `Shared` : briques transverses minimales réutilisées entre modules.

### 1.3 Dépendances observées entre projets
- `Domain` dépend de `Shared.Domain`.
- `Application` dépend de `Domain`, parfois aussi de `Shared.Application` et `Shared.Domain`.
- `Infrastructure` dépend de `Application`, `Domain`, `Shared.*`.
- le point d'entrée dépend de `Infrastructure`.

### 1.4 Règle de dépendance inférée
Le flux de dépendance est globalement centripète vers le domaine métier.

### 1.5 Variantes observées
- `Cotisation.Api` référence aussi `Affiliation.Infrastructure`, ce qui montre qu'un point d'entrée peut composer plusieurs modules.
- `Affiliation.Application` enregistre `Shared.Application`, alors que `Cotisation.Application` n'appelle pas `AddSharedApplicationLayer()`. Le besoin n'est donc pas uniformisé.

## 2. Conventions de nommage

### 2.1 Nommage des projets et namespaces
Observé :
- `Cnss.<Module>.<Layer>`
- exemples : `Cnss.Affiliation.Domain`, `Cnss.Cotisation.Application`

### 2.2 Nommage des dossiers
Observé :
- `Aggregats`
- `Entities`
- `ValuesObject`
- `Events`
- `Repositories`
- `Services`
- `Factories`
- dossiers applicatifs nommés par use case

Variante :
- `Aggregats` est orthographié ainsi partout dans le repo au lieu de `Aggregates`.
- `ValuesObject` est aussi utilisé au lieu de `ValueObjects`.

Décision :
- le socle mémoire et le futur skill conservent `Aggregats` et `ValueObject` comme conventions de référence, afin de rester alignés avec le style pédagogique retenu.

### 2.3 Nommage des types
Observé :
- entités/agrégats : noms métier singuliers (`Employer`, `Employee`, `Declaration`)
- repository interface : `I<Concept>Repository`
- handlers : `<UseCaseName>Handler`
- request/response : `<UseCaseName>Request`, `<UseCaseName>Response`
- validator : `<UseCaseName>RequestValidator`
- événements : `<PastTenseName>Event`
- factory : `<AggregateName>Factory`

### 2.4 Nommage technique des tables et colonnes
Observé :
- schéma par module : `affiliation`, `cotisation`
- préfixe métier court sur les tables/colonnes : `aff_`, `cot_`
- noms de tables en snake_case pluriel : `aff_employers`, `cot_declarations`
- noms de colonnes en snake_case longues et explicites

Hypothèse :
- le préfixe de persistance est lié au bounded context et devrait rester stable par module.

## 3. Représentation des entities

### 3.1 Base partagée
Observé :
- les entités héritent de `Entity<TId>`
- `Id` est porté par la classe de base
- égalité basée sur l'identifiant

### 3.2 Style observé
- identifiants métier de type `string`
- alias métier exposé via propriété calculée (`Identifier => Id`) dans plusieurs agrégats
- setters privés
- constructeur sans paramètre privé pour EF Core
- logique métier et validations simples directement dans le domaine

### 3.3 Exemples observés
- `Employee : AggregateRoot<string>`
- `DeclarationItem : Entity<string>`

### 3.4 Limites observées
- les entités ne portent pas de versioning, horodatage ou statut générique transversal.

## 4. Value Objects

### 4.1 Présence observée
Observé surtout dans `Cotisation.Domain` :
- `EmployerIdentifier`
- `DeclarationPeriod`
- `DeclarationData`
- `DeclarationItemData`

### 4.2 Implémentation observée
- héritage d'une base `ValueObject`
- égalité structurelle via `GetEqualityComponents()`
- validation au constructeur
- immutabilité pratique via propriétés sans setter public ou getter only

### 4.3 Variantes observées
Deux bases de `ValueObject` coexistent dans `Shared.Domain` :
- `Cnss.Shared.Domain.ValuesObject.ValueObject`
- `Cnss.Shared.Domain.Abstractions.ValueObject`

Observé :
- les value objects du repo utilisent la variante `Cnss.Shared.Domain.ValuesObject.ValueObject`
- aucune classe métier observée n'utilise `Cnss.Shared.Domain.Abstractions.ValueObject`

Hypothèse :
- `Abstractions/ValueObject.cs` est un artefact résiduel ou une ancienne variante.

Décision :
- la convention officielle du futur skill est d'hériter de `Cnss.Shared.Domain.Abstractions.ValueObject`.
- le fait que le repo d'exemple utilise l'autre variante doit être documenté comme écart historique, pas reproduit comme norme.

## 5. Aggregates

### 5.1 Base observée
- héritage de `AggregateRoot<TId>`
- liste interne `_domainEvents`
- exposition en lecture seule de `DomainEvents`

### 5.2 Style observé
- agrégat racine scellé (`sealed`)
- constructeur sans paramètre privé pour EF
- constructeur métier privé ou interne
- méthodes de fabrique statiques ou factory dédiée
- méthodes de restauration dédiées pour reconstruire un état sans publier d'événement

### 5.3 Exemples observés
- `Employer.Affiliate(...)`
- `Employer.Restore(...)`
- `DeclarationFactory.Create(...)` puis `Declaration.Publish()`
- `Declaration.Restore(...)`

### 5.4 Frontière d'agrégat observée
- `Employer` tient la liste des identifiants employés, pas une collection d'objets suivis en permanence
- `Declaration` contient explicitement ses `Items`

Hypothèse :
- le repo privilégie des agrégats compacts avec références par identifiants lorsque la cohérence immédiate sur les objets enfants n'est pas nécessaire.

## 6. Domain Services

### 6.1 Présence observée
Observé dans `Affiliation` :
- `IdentifierGenerator`

### 6.2 Rôle observé
- génération d'identifiants métier techniques `EMP-*`, `SAL-*`

### 6.3 Variante observée
- `Cotisation` n'emploie pas un domain service pour l'identifiant ; la logique est dans `DeclarationFactory`.

Décision :
- les identifiants métier doivent être générés par un domain service dédié.
- les identifiants techniques internes non métier doivent être gérés par la base de données, de préférence via auto-incrément.

## 7. Factories de domaine

### 7.1 Présence observée
Observé dans `Cotisation` :
- `DeclarationFactory`

### 7.2 Rôle observé
- transformer un objet de données métier (`DeclarationData`) en agrégat complet
- générer identifiants agrégat/enfants
- assembler plusieurs objets du domaine avant publication

### 7.3 Variante observée
- `Affiliation` préfère des méthodes statiques sur les agrégats.

Décision :
- l'usage d'une factory pour constituer un agrégat n'est pas systématique.
- il dépend de la contrainte conceptuelle du module et doit être décidé au cas par cas.
- une factory est pertinente lorsque l'assemblage ou les préconditions dépassent ce qu'une simple méthode statique rend lisible.

## 8. Repositories

### 8.1 Contrats observés
Les interfaces vivent dans `Domain/Repositories`.

Exemples :
- `IAffiliationRepository`
- `IDeclarationRepository`

### 8.2 Style observé
- signatures asynchrones
- `CancellationToken` optionnel
- nommage parfois orienté agrégat (`AddAsync`, `GetAsync`), parfois orienté cas métier (`AddEmployerAsync`, `GetEmployerAsync`)

### 8.3 Implémentations observées
- une implémentation EF Core par module dans `Infrastructure/Repositories`
- `SaveChangesAsync` est déclenché dans chaque méthode de repository

Variante :
- le repo n'impose pas encore d'unité de travail explicite partagée.

Risque observé :
- certaines opérations multi-écritures applicatives s'appuient sur plusieurs appels repository avec plusieurs `SaveChangesAsync`, donc sans transaction applicative explicite.

Décision :
- dans le projet métier cible, les repositories devront exposer un mécanisme de validation transactionnelle explicite.
- forme recommandée : `Commit()` ou `CommitAsync(bool flush = true)`.
- objectif : permettre de contrôler le moment du commit transactionnel, en particulier pour les use cases qui manipulent plusieurs repositories ou plusieurs agrégats.
- le skill doit donc proposer les opérations de persistance métier séparément des opérations de commit, même si le repo d'exemple ne le fait pas encore.

## 9. Use cases / Commands / Queries / Handlers

### 9.1 Structure observée
Un dossier par use case contenant en général :
- `...Request`
- `...Response`
- `...Handler`
- `...RequestValidator`

### 9.2 Mécanique observée
- `Request` implémente `IMDiatorRequest<TResponse>`
- `Handler` implémente `IMDiatorHandler<TRequest, TResponse>`
- validation explicite au début du handler via `ValidateAndThrowAsync`

### 9.3 Style métier observé
- le code n'étiquette pas explicitement `Command` ou `Query` dans les noms
- les tâches applicatives ressemblent aux use cases métier

Hypothèse forte pour le futur skill :
- la notion CNSS de `tâche` peut être mappée prioritairement vers le pattern de dossier/use case observé ici.

### 9.4 Orchestration inter-modules observée
`SubmitDeclarationHandler` appelle d'autres use cases via `IMediator` et `Shared.Application` :
- `GetEmployerDetailsRequest`
- `GetEmployerEmployeesDetailsRequest`

Convention observée :
- les contrats de lecture inter-modules sont exposés dans `Shared.Application`.

## 10. Domain Events

### 10.1 Représentation observée
- record scellé héritant de `DomainEvent`
- horodatage `OccurredOn` fourni par la base

### 10.2 Déclenchement observé
- ajout explicite depuis les agrégats via `AddDomainEvent(...)`
- purge via `ClearDomainEvents()`

### 10.3 Convention de nommage observée
- temps passé métier : `EmployerAffiliatedEvent`, `DeclarationPublishedEvent`

### 10.4 Exploitation technique observée
- interception EF Core avant sauvegarde
- sérialisation JSON de l'événement
- publication RabbitMQ via outbox
- routing key dérivée du nom d'événement en kebab-case préfixé par le boundary : `affiliation.employer-affiliated`, `cotisation.declaration-published`

## 11. DTOs / Contrats d'entrée-sortie

### 11.1 DTOs applicatifs observés
- records en `Application`
- shape minimale, sans logique

### 11.2 DTOs d'adaptation observés
- en API, un contrat HTTP spécifique peut être défini puis converti vers un `Request` applicatif
- exemple : `SubmitDeclarationHttpRequest`

### 11.3 DTOs partagés observés
- `Shared.Application` contient des contracts de requêtes/réponses inter-modules

## 12. Règles de dépendances

### 12.1 Observé
- le domaine ne dépend pas de l'infrastructure
- l'application dépend des abstractions de domaine
- l'infrastructure référence l'application pour composer l'ensemble
- le point d'entrée ne parle pas au domaine directement

### 12.2 Tolérances observées
- le point d'entrée peut agréger plusieurs bounded contexts

### 12.3 Hypothèses
- le futur skill doit privilégier ces règles plutôt qu'une Clean Architecture plus "pure" non observée.

## 13. Persistance et mapping EF Core

### 13.1 Observé
- un `DbContext` par module
- schéma SQL dédié par module
- configuration fluent dans `OnModelCreating`
- seed data minimale dans les `DbContext`
- migrations sous `Infrastructure/Persistence/Migrations`

### 13.2 Conventions observées
- ignorer les propriétés dérivées du domaine (`Identifier`, `TotalAmount`, `DomainEvents`)
- utiliser `OwnsOne` pour certains value objects
- utiliser `PropertyAccessMode.Field` pour les collections encapsulées

### 13.3 Variante observée
- `AffiliationRepository.GetEmployerAsync()` reconstruit explicitement l'agrégat avec `Restore(...)`
- `DeclarationRepository.GetAsync()` retourne l'agrégat EF directement avec `Include(x => x.Items)`

Décision :
- la constitution d'un agrégat via `Restore(...)`, factory ou matérialisation plus directe dépend de la contrainte conceptuelle et du niveau d'encapsulation recherché.
- le futur skill ne doit pas imposer une factory ou une méthode `Restore(...)` de manière systématique.

## 14. Intégration / Messaging

### 14.1 Observé
- outbox par module
- type de record d'outbox spécifique au module
- processor hébergé par module
- publisher RabbitMQ spécialisé par module mais hérité d'une implémentation commune

### 14.2 Conventions observées
- `OutboxMessageStatus` simple par chaînes
- retry basique via `AttemptCount`
- verrou applicatif léger via `LockedUntilUtc`

## 15. Tests

### 15.1 Observé
- tests unitaires ciblés sur le comportement métier et l'orchestration applicative
- peu de tests d'intégration observés

### 15.2 Style observé
- nommage `Method_Should_ExpectedBehavior`
- fake repository / fake mediator écrits localement au test

## 16. Erreurs fréquentes à éviter pour rester conforme au style observé

- mettre de la logique métier principale dans l'API, le Portal ou le `DbContext`
- faire dépendre le domaine de l'infrastructure
- exposer des setters publics sur les agrégats et value objects
- omettre la méthode de validation applicative avant exécution du handler
- publier des domain events depuis l'application au lieu de l'agrégat
- oublier d'ignorer `DomainEvents` et propriétés dérivées dans EF
- confondre contrat HTTP et contrat applicatif
- supposer une convention `Command/Query` dans les noms alors qu'elle n'est pas observée
- imposer des identifiants GUID purs alors que les identifiants métier préfixés sont la forme observée
- effacer une variante existante du repo sans la documenter

## 17. Conventions observées à reprendre telles quelles dans un skill

- dossier par use case
- `Request/Response/Handler/Validator`
- agrégat scellé avec constructeur privé pour EF
- méthodes `Restore(...)` lorsqu'une reconstruction explicite est nécessaire
- domain events en records
- interfaces de repositories dans `Domain`, implémentations dans `Infrastructure`
- conventions de dossiers `Aggregats` et `ValueObject`
- `DbContext` par module avec schéma SQL dédié
- outbox par bounded context
- contrats partagés inter-modules sous `Shared.Application`
- séparation forte entre modèle métier et adaptation HTTP/UI

## 18. Hypothèses raisonnables pour un futur repo mémoire CNSS

- un module mémoire CNSS pourra correspondre à un bounded context principal ou à un sous-ensemble stable d'un bounded context.
- une `tâche` issue de l'application existante est un bon candidat de use case tant qu'aucune preuve ne montre l'inverse.
- les lois, règlements et manuels alimenteront d'abord les règles métier, puis les use cases, puis les agrégats.
- l'incrémentation des modèles devra préserver l'existant validé et produire des écarts explicites, car la continuité fonctionnelle est un objectif déclaré.

## 19. Décisions de cadrage déjà actées pour le futur skill

- conserver `Aggregats` comme convention de dossier et de vocabulaire structurel
- conserver `ValueObject` comme convention de vocabulaire structurel
- utiliser `Cnss.Shared.Domain.Abstractions.ValueObject` comme base officielle des futurs value objects
- générer les identifiants métier via un domain service
- laisser les identifiants techniques non métier à la base de données via auto-incrément
- autoriser factory, méthode statique ou restauration explicite selon la contrainte conceptuelle du module
- introduire un `Commit()` ou `CommitAsync(bool flush = true)` dans les repositories du projet métier cible pour supporter le contrôle transactionnel

## 20. Points restant à clarifier avant industrialisation complète

- niveau de granularité cible d'un module CNSS mémoire
- statut officiel des contrats partagés analogues à `Shared.Application` quand plusieurs contextes consomment le même modèle
- vocabulaire officiel à employer entre `module`, `bounded context`, `sous-domaine`, `tâche`, `use case`
