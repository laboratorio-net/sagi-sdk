# Sagi.Sdk.Results

SDK para padronização de retornos e tratamento de resultados em operações .NET.

## Instalação

```sh
dotnet add package Sagi.Sdk.Results
```

## Como Usar

```csharp
using Sagi.Sdk.Results;

Result resultado = MinhaOperacao();
if (resultado.IsSuccess)
{
    // Sucesso
}
else
{
    Console.WriteLine(resultado.Error);
}
```

---