# 🌴 Sagi SDKs

## O que é o Sagi?

Sagi é uma coleção de SDKs desenvolvidos em .NET para facilitar a integração e implementação de soluções comuns em aplicações modernas. Cada SDK é modular e segue boas práticas de design, garantindo flexibilidade e reutilização de código.

## Significado Original  

"Sagi" é o nome de uma praia localizada no extremo sul do Rio Grande do Norte, na divisa com a Paraíba. Ela faz parte do município de Baía Formosa e é conhecida por suas belezas naturais, rios, manguezais e um clima tranquilo, sendo um destino pouco explorado pelo turismo de massa.  

Sobre o significado do nome em si, não há um consenso, mas pode ter origem indígena, como muitos nomes de praias e cidades do Nordeste. Alguns acreditam que "Sagi" tenha relação com palavras tupi ligadas à água ou natureza, o que faz sentido considerando a geografia do local.

## Por que "Sagi"?  

Porque amo morar no Rio Grande do Norte e acho incríveis os nomes das praias, cada uma mais linda do que a outra. Por isso, resolvi homenagear o lugar que amo batizando este projeto com o nome de uma de suas praias.

---

## SDKs Disponíveis  

O projeto Sagi é composto por vários SDKs independentes, cada um focado em uma funcionalidade específica:

- **Sagi.Sdk.Mongo** – Integração e abstração para o MongoDB.  
- **Sagi.Sdk.SqlServer** – Implementação para uso com SQL Server.  
- **Sagi.Sdk.Identity** – Gerenciamento de identidade e autenticação.  
- **Sagi.Sdk.Entities** – Modelos de entidades padronizados.  
- **Sagi.Sdk.ValueObjects** – Implementação de objetos de valor imutáveis.  

---

## Como Instalar  

Cada SDK pode ser instalado separadamente via NuGet. Exemplo de instalação via CLI:
```sh
dotnet add package Sagi.Sdk.Mongo
```

## Como Usar

Exemplo de Uso com `Sagi.Sdk.Mongo`

```c#
using Sagi.Sdk.Mongo;

var repository = new MongoRepository<MyEntity>("connectionString", "databaseName");
var entity = await repository.GetByIdAsync("entityId");

```

Para mais exemplos e documentação completa, veja os repositórios individuais de cada SDK.

---
## Contribuindo

Contribuições são bem-vindas! Se você deseja melhorar ou adicionar novas funcionalidades, siga os passos:

1. **Faça um fork** do repositório.    
2. **Crie uma branch** para sua feature (`git checkout -b feature/nova-feature`).    
3. **Implemente sua alteração** seguindo as boas práticas do .NET.    
4. **Envie um Pull Request** para análise.

## Licença

Este projeto é distribuído sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.
