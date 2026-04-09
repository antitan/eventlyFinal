# Evently — Architecture Overview

## Vue d’ensemble

Evently est un **modular monolith** .NET centré sur quatre modules métiers (`Users`, `Events`, `Ticketing`, `Attendance`) et un noyau commun (`Common`).

- Un seul process API (`Evently.Api`) héberge tous les modules.
- Chaque module respecte une séparation en couches: `Domain`, `Application`, `Infrastructure`, `Presentation`.
- Les échanges inter-modules sont faits via des **IntegrationEvents** + patterns **Outbox/Inbox** pour la fiabilité et l’idempotence.

## Composition racine

Le bootstrap principal est dans `src/API/Evently.Api/Program.cs`:

1. Chargement de la couche application commune (MediatR + validators + pipeline behaviors).
2. Chargement de l’infrastructure commune (auth, authz, cache, bus, OpenTelemetry, Quartz).
3. Chargement des fichiers de config par module (`modules.*.json`).
4. Enregistrement de chaque module (`AddUsersModule`, `AddEventsModule`, etc.).
5. Mapping auto des endpoints minimal API via `IEndpoint`.

## Couches et responsabilités

### Domain

Contient les entités, règles métier et domain events.

Exemple:
- `Event` expose `Create`, `Publish`, `Reschedule`, `Cancel`.
- Les changements significatifs lèvent des events (`EventCreatedDomainEvent`, etc.).

### Application

Contient use-cases (commands/queries) et orchestration:

- Contrats MediatR (`ICommand`, `IQuery`).
- Handlers qui orchestrent repositories + UnitOfWork.
- Validation via FluentValidation.

Pipelines transverses:
- `ExceptionHandlingPipelineBehavior`
- `RequestLoggingPipelineBehavior`
- `ValidationPipelineBehavior`

### Infrastructure

Implémentations techniques:

- EF Core `DbContext` par module + repositories + `IUnitOfWork`.
- SQL Server (principal), Redis (cache/saga), Dapper (jobs Inbox/Outbox).
- MassTransit pour bus interne.
- Quartz pour exécution périodique des jobs inbox/outbox.

### Presentation

Endpoints HTTP minimal API:

- Chaque endpoint implémente `IEndpoint`.
- Auto-découverte/auto-mapping via `AddEndpoints` + `MapEndpoints`.
- Hand-off vers MediatR (`ISender`).
- Conversion des `Result` métier en réponses HTTP (`ApiResults.Problem`).

## Communication inter-modules

### Domain → Outbox

Pendant `SaveChanges`, l’intercepteur `InsertOutboxMessagesInterceptor`:

1. Récupère les `DomainEvents` des entités suivies.
2. Les sérialise en `outbox_messages`.
3. Les persiste dans la **même transaction** que les données métier.

### Outbox job

Un job Quartz `ProcessOutboxJob` lit les messages `outbox_messages` non traités, les désérialise puis invoque les handlers de domain events. Les erreurs sont journalisées et stockées dans la table.

### Integration events + Inbox

Les consumers MassTransit écrivent les événements entrants en `inbox_messages`.
Ensuite `ProcessInboxJob` lit inbox, désérialise, exécute les handlers d’intégration du module et marque les messages comme traités.

### Idempotence

Les handlers domain/integration sont décorés avec:

- `IdempotentDomainEventHandler<>`
- `IdempotentIntegrationEventHandler<>`

Les tables de tracking (`outbox_message_consumers` / `inbox_message_consumers`) empêchent les re-traitements.

## Sécurité

- Auth JWT Bearer via `JwtBearerConfigureOptions` depuis la config.
- Autorisation dynamique par permissions:
  - `PermissionAuthorizationPolicyProvider` génère les policies à la volée.
  - `PermissionAuthorizationHandler` vérifie les claims de permissions.

## Observabilité

- Logging: Serilog.
- Tracing: OpenTelemetry (ASP.NET Core, HttpClient, EF Core, Redis, MassTransit).
- Export OTLP configurable.
- Health checks: Redis + Keycloak.

## Contrôle d’architecture

Des tests NetArchTest garantissent:

- Aucune dépendance interdite entre couches d’un module.
- Pas de dépendances directes inter-modules (hors assemblies d’IntegrationEvents).

## Dépendances runtime (docker-compose)

- API ASP.NET Core
- SQL Server 2022
- Keycloak
- Redis
- Seq
- Jaeger

## Subtilités importantes

1. **Module boundaries strictes**: un module ne doit pas consommer directement un autre module.
2. **Outbox/Inbox**: permet robustesse et découplage asynchrone tout en restant monolithe.
3. **CQRS pragmatique**: Commands/Queries séparés avec MediatR, sans microservices.
4. **Mapping endpoint automatique**: pas de gros controller central, endpoints distribués par feature.
5. **Fallback cache**: Redis indisponible => cache mémoire pour faciliter les environnements locaux.
