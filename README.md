Pra entendermos o panorama total de Autenticação e Autorização, precisamos entender todo o contexto que envolve isso, e, o ponto inicial de largada é entender o que são os famosos **tokens JWT** que garantem a segurança no trafego de informações via JSON por aplicações.

## JsonWebToken (JWT)

O JWT nada mais é que **um padrão (RFC-75129) de mercado que define como transmitir e armazenar objetos JSON de forma compacta e segura entre diferentes aplicações.**

Os dados contidos dentro de um JWT podem ser validados a qualquer momento, **pois o token é assinado digitalmente.**

Dentro do entendimento dos Tokens JWT temos que saber a importancia de alguns topicos como:

## Header

O Header é um objeto JSON que vai definir **informações sobre o tipo do token (typ), e o algoritmo de criptografia que foi usado na assinatura (alg), que geralmente são criptgrafias baseadas em HMAC SHA256 ou RSA**

```
{
  "alg": "HS256,
  "typ": "JWT"
}
```

## Payload
É um Objeto JSON que contem as claims (informações) da entidade tratada em questão. Normalmente é o usuário autenticado.
Existem 3 tipos de claims:

* Reserved Claims: Atributos não obrigatorios (mas recomendados) que são utilizados na validação do token pelos protocolos de segurança de APIs
* Public Claims: Atributos que usamos em nossas aplicações. Normalmente armazenamos informações do usuário autenticado na aplicação.
```
name
roles
permissions
```
* Private Claims: Atributos definidos especialmente para compartilhar informações entre aplicações
```
 {
    "sub":"12323232",
    "name":"John Doe",
    "admin": true
 }
```

## Signature
A Assinatura (signature) é a concatenação dos HASHES gerados a partir do Header e do Payload, usando base64UrlEncode, com uam chave secreta ou certificado RSA. Logo, essa assinatura vai garantir a integridade do token, pq se ele for modificado no meio do caminho de uma request, terá como sabermos.
Isso previne ataques, pois se o Payload for alterado, o hash final não será válido, pq nã foi assinado com a chave correta

## Resultado Final
O Resultado final é um token com 3 sessões (Header, Payload, Signature) separadas todas por um ponto

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

## Usando o token
Quando formos fazer login em um serviço de autenticação, um token JWT vai ser criado e retornado para o cliente. Esse token vai ser enviado para APIs através do Header **Authorization** de cada request HTTP, com a flag **Bearer**, conforme abaixo:

````
{
  "Authorization": "Bearer <tokenAqui>
}
````
E essa comunicação entre cliente/servidor de autorização/api funciona assim:

```
1 - Cliente dá um POST numa rota (ex: /users/login) com o usuario e senha
2 - No Servidor, o JWT vai ser criado com seu secret
3 - O Servidor vai retornar para o cliente o token JWT
4 - O Cliente manda o token JWT recebido no Header Authorization da request pro servidor
5 - O Servidor vai checar a assinatura JWT que o cliente acabou de mandar
6 - O Servidor depois de checar a assinatura irá mandar uma resposta para o cliente confirmando se o token foi aceito ou não
```

Com isso, **a API, em posse do token, não precisa ir até o banco de dados consultas as infos do usuario, pq dentro do proprio token JWT já vai estar contida as credenciais de acesso.**

O diagrama a seguir mostra em bem alto nivel como funciona a obtenção de um token JWT do cliente para o servidor, e do cliente para a aplicação:

````
1 - Cliente Mada requst pro servidor OAuth
2 - Servidor OAuth retorna o token JWT
3 - Cliente usa esse token JWT e obtem acesso à recursos na API
````
## Itens de um token
- **Aud**: Essa Claim do Token JWT refere aos grupos de recursos que devem aceitar esse token
- **client_Id**: Refere a aplicação cliente que está requisitando recursos do grupo de recursos.
- **scope**: Um Scope é o mecanismo para limitar o acesso da aplicação para uma conta de usuario. Uma aplicação pode solicitar um ou mais escopos, e essa informação é representada para o usuario na tela, e o **token de acesso recebido vai ser limitado aos scopes apresentados.**
- **iss**: Essa claim identifica quem pediu o JWT. O Processamento dessa claim geralmente é uma aplicação específica. Esse valor ‘iss’ é case sensitive e contem um valor Uri, seu uso é opcional

README Em trabalho... Novas anotações serão adicionadas 
