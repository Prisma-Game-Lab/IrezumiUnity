READ ME DIALOG BOX 

Para utilizar a dialog que INICIA JUNTO COM A FASE:
1- arraste o dialogCanvas de Prefabs para a tela
2- No inspector>Canvas>Render Camera escolha a camera da cena
3- No inspector>DialogBox
	3.1- Em text file escolha um .txt para ser seu dialogo
	3.2- Em Font selecione sua fonte
	3.3- Font Size é a fonte do texto e Names Size é a fonte do nome do personagem
	3.4- Selecione ScrollText para fazer letra por letra aparecer na tela
	3.5- Cps é a velocidade de letras por segundo 
	3.6- Scale Factor é o quanto a area com a imagem do personagem aumenta quando ele está falando

*Para utilizar um dialog que é acionado quando voce entra em uma area utilizar o prefab ShoutZone
	
Lista de comandos possiveis para a dialog box txt:

=====================================================================
Nome: 		Talking
O que faz: 	aumenta a imagem de uma das tres areas de imagem(centro, direita e esquerda)
Comando:	talking left,right,center

Exemplos: 
talking center
talking none

Obs: 
- para voltar as tres areas para o tamanho normal basta escrever talking none
=====================================================================
Nome: 		Fixed
O que faz: 	Coloque este comando antes de uma frase para não deixar que o player pule o dialogo clicando em qualquer tecla
Comando:	[fixed]

Exemplos:
[fixed] Voce não pode pular esse dialogo hehe
=====================================================================
Nome: 		Pula linha
O que faz: 	pula uma linha
Comando:	<\n>

Exemplos:
Era voce!<\n>
Agora nós sabemos em que confiar
=====================================================================
Nome: 		Show
O que faz: 	mostra na tela a imagem desejada em uma das tres areas de imagem(centro, direita e esquerda) 
Comando:	show left,right,center nomeimagem

Exemplos:
show right none
show center Hideaki2

Obs: 
- para tirar uma imagem do lugar basta trocar o nomeimagem por none
- nomeimagem deve ser uma imagem que pertence a pasta resources/visualnovel
- nao precisa de extensao no nomeimagem(.png,.jpg etc) (precisa da extensao na imagem da pasta)
=====================================================================
Nome: 		Cps (character per second)
O que faz: 	muda a velocidade com que as letras sao escritas na tela 
Comando:	<cps = num> </cps>

Exemplos:
Oi, meu nome é <cps=2> ruda </cps>

Obs: 
- existe um cps default (inspector) que é a velocidade normal de todas as letras
=====================================================================
Nome: 		Wait
O que faz: 	pausa a escrita no tempo determinado (num)
Comando: 	<w = num>

Exemplos:
Eu nao sei o que dizer<w=1> .<w=0.5>.<w=0.5>.<w=0.5>
=====================================================================
