# AI Sandbox Engine

Универсальное .NET-ядро для детерминированных игровых симуляций с AI-управляемыми персонажами, субъективным восприятием мира, памятью, знаниями, отношениями и безопасной интеграцией языковых моделей.

Проект отделяет **авторитетное состояние мира** от недоверенного вывода AI-моделей. Модель может предлагать действия и формировать реплики, но не может напрямую изменять игровой мир.

## Статус

**Core завершён на этапе `0094 Core Product Pipeline Completion` и используется как стабильная основа для интеграции с играми.**

Финальная продуктовая проверка связывает существующие подсистемы в один исполняемый pipeline:

```text
Context Retrieval
    ↓
Prompt Budget + Prompt Composition
    ↓
Provider-neutral Model Invocation
    ↓
Structured Output
    ↓
Action Validation
    ↓
Runtime Command
    ↓
Authoritative World State transition
```

До выполнения подтверждённой команды AI-часть pipeline не изменяет World State. Финальный принятый command выполняет единственный авторитетный переход состояния.

По последней зафиксированной полной проверке Core использовал baseline из **782 автоматических тестов** вместе с исполняемым `FoundationProbe` и отдельным completion-validator.

Дальнейшая разработка Core выполняется только при обнаружении конкретной ошибки, интеграционного блокера или отсутствующей продуктовой возможности. Игровая интеграция ведётся в отдельном Unreal Engine 5 проекте `AI-Sandbox-Detective`.

## Основные принципы

- **World State — единственный источник объективной истины.**
- **LLM output недоверенный и неавторитетный.** Любое предлагаемое действие проходит типизированную валидацию.
- **Commands и Events разделены.** Command — запрос на изменение, Event — уже произошедший факт.
- **Детерминированность является частью архитектуры.** Логическое время, случайность, порядок систем и авторитетные переходы состояния задаются явно.
- **NPC не получают глобальное знание автоматически.** Perception, Knowledge, Memory и Relationships существуют как отдельные слои.
- **Provider-neutral Core.** Конкретные LLM/STT/TTS SDK, transport и credentials остаются за пределами ядра.
- **Gameplay и presentation зависят от Core, но Core не зависит от конкретной игры или движка.**

## Архитектура

Концептуальные уровни:

```text
Infrastructure
    ↓
Data
    ↓
Runtime
    ↓
Simulation
    ↓
AI-facing contracts
    ↓
Gameplay / Integration
    ↓
Presentation
```

Авторитетный путь изменения мира:

```text
External input / simulation event
    ↓
Intent or proposed action
    ↓
Validation
    ↓
Authoritative state transition
    ↓
Domain events
```

### Базовая симуляция

Core включает:

- strongly typed identifiers;
- immutable domain events;
- World State с версиями и проверяемыми переходами;
- entity lifecycle и typed components;
- deterministic simulation scheduler;
- snapshot persistence и восстановление;
- version-gated typed commands;
- deterministic random streams;
- integer fixed-step simulation time;
- caller-driven runtime orchestration;
- иерархическую spatial model.

### Субъективное состояние NPC

Отдельные подсистемы моделируют:

- **Perception** — что конкретный наблюдатель способен воспринять;
- **Knowledge** — текущие субъективные сведения персонажа;
- **Memory** — сохранённые эпизоды с provenance, salience и strength;
- **Relationships** — направленное состояние отношений между сущностями;
- **Context Retrieval** — ограниченную детерминированную выборку релевантного контекста.

Perception не превращается автоматически в объективный факт, Knowledge или Memory.

### AI и диалоги

Core содержит provider-neutral контракты и процессоры для:

- prompt budgeting;
- prompt composition;
- model invocation;
- speech recognition/synthesis boundaries;
- conversation state;
- semantic address resolution;
- social turn-taking;
- structured model output;
- dialogue orchestration;
- behavior intent и action validation.

Конкретный провайдер модели, STT/TTS, transport и игровой bridge реализуются внешними адаптерами.

### Host Runtime

Host Runtime описывает переносимую authority-модель для выполнения внешней работы:

- lifecycle и health;
- request correlation и cancellation intent;
- deadlines и retry decisions;
- queue admission и priority;
- worker ownership leases;
- dispatch/completion routing;
- retry/requeue/dead-letter outcomes;
- active-work reconciliation;
- checkpoint/recovery authority и bounded recovery queries.

Core не запускает фоновые worker-процессы и не владеет конкретной очередью или transport. Он задаёт проверяемые типизированные контракты, которыми может пользоваться внешний Host.

## Структура репозитория

```text
src/
  AI.Sandbox.Engine.Core/          основное универсальное ядро

tests/
  AI.Sandbox.Engine.Core.Tests/    автоматические тесты

samples/
  AI.Sandbox.Engine.FoundationProbe/
                                   headless integration/product probes

eng/
  build.ps1                        Release/Debug build
  test.ps1                         полный test pipeline
  validate-foundation.ps1          детерминизм + foundation probe
  validate-core-completion.ps1     финальный product pipeline gate
  verify-repository.ps1            архитектурные и repository invariants

docs/
  architecture/                    ADR и архитектурные решения
  product/                         продуктовая модель
  roadmap/                         история foundation-разработки
  validation/                      критерии foundation validation
```

## Требования

- Windows, Linux или macOS с .NET SDK, совместимым с проектом;
- **.NET 10 SDK** согласно `global.json`;
- PowerShell 7+ для стандартных `eng/*.ps1` entry points.

Основной target framework:

```text
net10.0
```

Проект использует nullable reference types, warnings-as-errors, deterministic builds и centrally managed NuGet package versions.

## Сборка

Из корня репозитория:

```powershell
& .\eng\build.ps1
```

По умолчанию выполняется Release build с locked restore.

## Тестирование

Полный build + test pipeline:

```powershell
& .\eng\test.ps1
```

Foundation validation с проверкой повторяемого checksum:

```powershell
& .\eng\validate-foundation.ps1
```

Финальный Core completion gate:

```powershell
& .\eng\validate-core-completion.ps1
```

Completion validator проверяет repository invariants, Release build, полный test baseline и исполняемый product-shaped pipeline.

## FoundationProbe

`AI.Sandbox.Engine.FoundationProbe` — headless consumer Core. Он используется для проверки того, что ключевые подсистемы действительно компонуются и выполняются вместе без зависимости от Unreal Engine или конкретного AI-провайдера.

В финальной product pipeline проверке используются:

1. bounded context retrieval;
2. prompt composition;
3. детерминированный fake model adapter;
4. structured output decoding;
5. action validation;
6. runtime command commit.

Fake adapter является только validation scaffolding и не задаёт конкретного production-провайдера.

## Интеграция с игрой

Первая игровая интеграция — отдельный Unreal Engine 5 проект **AI-Sandbox-Detective**.

Общая схема:

```text
Unreal Engine 5
    ↕
local bridge / Host
    ↕
AI Sandbox Engine Core
    ↕
LLM / STT / TTS / persistence adapters
```

Такое разделение позволяет тестировать симуляцию headless, менять AI-провайдеров без зависимости Core от их SDK и изолировать game/presentation-specific код от универсальной логики.

## Что намеренно не входит в Core

- конкретные OpenAI, Anthropic, Gemini, Ollama и другие model SDK;
- конкретные STT/TTS-провайдеры;
- Unreal/Unity types;
- UI и presentation;
- сетевой transport;
- DI container;
- реальные worker queues и background services;
- gameplay-specific detective или quest logic;
- credentials и локальная конфигурация.

Эти части подключаются на уровне Host, adapters или конкретной игры.

## Технологии

- C# / .NET 10
- xUnit
- immutable domain models
- deterministic simulation
- strongly typed contracts
- snapshot persistence
- provider-neutral AI boundaries
- PowerShell validation tooling
- Git

## Состояние Core

`0094 Core Product Pipeline Completion` является terminal Core gate. Архитектура не расширяется механически ради дополнительных обёрток или уровней. Новые изменения в Core должны быть обоснованы реальной интеграцией или исправлением поведения.
