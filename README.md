# BoletimNotasEscolares
Projeto Web que realiza uma carga de banco de dados com alunos e notas e permite baixar um relatório das mesmas, contendo a média de cada aluno, inclusive.
Este projeto foi escrito utilizando: C#, Asp.Net Core MVC, Html, Bootstrap, Css, Javascript, jQuery e Transact Sql (banco de dados Sql Server)

Para rodar o projeto localmente, os requisitos mínimos são:<br />
-Ter instalado o SDK do .Net Core versão 3.1<br />
-Ter um banco de dados Sql Server rodando na máquina, podendo inclusive ser a versão Express

Com os requisitos de instalação atendidos:
1. Baixe este repositório para algum drive e diretório de sua preferência
2. Abra um Prompt de Comando, alcance a pasta raíz da solução e digite as seguintes instruções:<br />
   dotnet run -- --CREATEDB=true --MASTERDBCONNSTR="Server=.;Initial catalog=master;Integrated security=SSPI" --ADMINUSERNAME="candidato-evolucional" --ADMINPASSWORD="123456" [ENTER]<br />
   dotnet run [ENTER]
3. Abra um navegador de Internet de sua preferência e acesse o seguinte endereço:
   https://localhost:5001
4. No menu superior do site, clique em "Administração". (Você será direcionado para a página de autenticação neste momento)
5. Na tela de autenticação, informe os seguintes valores nos campos Username e Password, respectivamente:
   candidato-evolucional
   123456
(você será redirecionado para a tela de administração através da qual você poderá realizar a carga de dados e baixar o relatório das notas dos alunos)

Teve algum problema ao realizar estes passos? Contate-me!
