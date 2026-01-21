Euterpe

Euterpe é um leitor de áudio moderno e leve, desenvolvido em C# e .NET 8, projetado para pessoas que possuem uma grande coleção de arquivos de áudio localmente. Ele permite organizar, navegar e reproduzir músicas de forma intuitiva, com suporte a múltiplos formatos, interface responsiva e controles completos de reprodução.

Funcionalidades

🎵 Suporte a diversos formatos de áudio: MP3, WAV, FLAC e M4A.

🖼 Visualização de álbuns: exibe capas e informações do álbum em um grid interativo.

🖥 Janela de álbum única: ao clicar em um álbum, a janela é atualizada com suas músicas, evitando conflitos de reprodução.

▶️ Controles de reprodução completos: play/pause, avançar, retroceder, barra de progresso e tempo de música.

💾 Persistência da pasta de músicas: lembra automaticamente da última pasta de áudio escolhida pelo usuário.

🌙 Interface escura: design moderno e confortável para longos períodos de uso.

📂 Organização de músicas por pastas: cada álbum corresponde a uma pasta no seu sistema de arquivos.

Como funciona

Escolha sua pasta de músicas
Ao abrir o Euterpe, selecione a pasta onde seus arquivos de áudio estão armazenados. A aplicação irá escanear todas as pastas dentro deste diretório e identificar cada álbum.

Visualize seus álbuns
Todos os álbuns encontrados são exibidos em um grid com a capa e o nome do álbum/artista.

Abra um álbum
Clique em um álbum para abrir a janela de faixas.

A janela é única e se atualiza ao selecionar outro álbum.

Nenhum conflito de áudio ocorre entre janelas diferentes.

O áudio só começa a tocar quando você clicar na faixa desejada.

Reproduza suas músicas

Controles de reprodução: Play/Pause, Próxima, Anterior.

Barra de progresso com tempo atual e total da faixa.

O nome da música e a capa do álbum só aparecem quando uma faixa é reproduzida.

Mudar a pasta de músicas
Através do menu “Arquivo → Alterar pasta de músicas”, você pode selecionar uma nova pasta.

O menu de seleção tem fundo escuro e fonte clara para manter o padrão da interface.

Tecnologias utilizadas

C# – linguagem principal do projeto.

.NET 8 – framework para desenvolvimento do aplicativo desktop.

WPF (Windows Presentation Foundation) – para interface gráfica.

MediaPlayer – para reprodução de arquivos de áudio locais.


Como rodar

Clone o repositório:

**git clone(https://github.com/EduardoGibertoni/Euterpe-Music-Player.git)**


Entre na pasta do projeto:

**cd euterpe**


Execute o projeto usando .NET:

**dotnet run**


**Requer Windows com .NET 8 SDK instalado.**
