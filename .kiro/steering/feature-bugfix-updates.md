---
inclusion: auto
---

# Feature & Bugfix Updates — Steering

Sempre que você implementar uma **feature** ou **bugfix**, você DEVE:

## 1. Atualizar README.md

Para cada SDK afetado, atualize o `README.md` correspondente:
- **Localização:** `{SDK}/src/{SDK}/README.md`
- **O que atualizar:**
  - Adicione a nova funcionalidade ou correção na seção apropriada
  - Inclua exemplos de uso se aplicável
  - Atualize a versão ou changelog se existir
  - Mantenha o tom consistente com o documento existente

## 2. Atualizar Steering Files

Atualize este arquivo (`.kiro/steering/project-context.md`) se:
- A mudança afeta a arquitetura ou estrutura de um SDK
- Novos padrões ou convenções foram introduzidos
- Dependências foram adicionadas ou removidas
- Algoritmos ou validações foram alterados significativamente

**Seções a considerar:**
- `## Projetos` — Descrição do SDK e seus arquivos principais
- `## Convenções de Código` — Se novas convenções foram estabelecidas
- `## Histórico de Alterações` — Adicione uma entrada com data, SDK, e resumo da mudança
- `## Dependências entre Projetos` — Se a topologia mudou

## 3. Formato de Entrada no Histórico

Quando adicionar ao `## Histórico de Alterações`:

```markdown
### [YYYY-MM] {SDK} — {Título da Mudança}

#### Contexto
Breve explicação do problema ou necessidade.

#### Mudanças
- Arquivo 1: descrição
- Arquivo 2: descrição

#### Decisões de Design
- Decisão 1 e justificativa
- Decisão 2 e justificativa

#### Padrão a Seguir
Se aplicável, descreva o padrão para futuras extensões similares.
```

## Exemplo

```markdown
### [2026-05] Sagi.Sdk.DotEnv — Novo SDK

#### Contexto
Necessidade de integrar arquivos `.env` ao pipeline de configuração nativo do .NET.

#### Mudanças
- `DotEnvParser.cs`: parsing de linhas com suporte a comentários
- `DotEnvProvider.cs`: herda ConfigurationProvider
- `ConfigurationBuilderExtensions.cs`: AddDotEnv() e AddDotEnv(Action<DotEnvOptions>)

#### Decisões de Design
- Sem dependências externas além de Microsoft.Extensions.Configuration
- Arquivo ausente não lança exceção (comportamento permissivo)

#### Padrão a Seguir
Novos SDKs devem seguir a mesma estrutura: `src/`, `tests/`, `README.md`.
```

## Checklist

Antes de considerar a feature/bugfix completa:

- [ ] Código implementado e testado
- [ ] README.md atualizado com exemplos
- [ ] Steering file atualizado (se aplicável)
- [ ] Entrada adicionada ao Histórico de Alterações
- [ ] Commit segue Conventional Commits
