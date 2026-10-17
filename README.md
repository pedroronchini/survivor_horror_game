# Untitled Survival Horror (working title)

<img width="1624" height="886" alt="devlog01" src="https://github.com/user-attachments/assets/cd9eacc7-c3e6-4cbb-9eb8-e009ac6d15ae" />

**English** · [Português](#português)

A third-person survival horror game in development with Unity 6, inspired by *Resident Evil 2 Remake*, *Silent Hill* and classic science fiction.

In a future where humanity has long abandoned Earth, humanoid robots that were never capable of harming anyone start killing their owners. A detective follows their trail to a forgotten city on the planet everyone left behind, where exploration, scarce resources and environmental storytelling matter more than firepower.

> **Status:** early prototype, greybox stage. The goal is to get the feel of moving, aiming and exploring right before adding art.

### Implemented so far

- **Over-the-shoulder camera** (Cinemachine 3): obstacle avoidance, aim zoom with FOV transition, and a neck-height camera target
- **Camera proximity fade**: hides the character (keeping its shadow) when the camera gets too close, with hysteresis to prevent flickering
- **Strafe-style movement**: the character faces the camera direction, with different speeds for forward, sideways and backward movement
- **Animation system**: two 2D Freeform Directional blend trees (locomotion and aiming) driven by the character's real velocity in local space
- **Editor tool** that generates the first city's greybox (streets, sidewalks, buildings, blockades and boundary walls) from a scaled level plan
- **Inventory** with stackable items built on ScriptableObjects
- **Interaction system** for picking up items
- **Pause menu** and aiming crosshair UI

### Tech

Unity 6 (6000.5) · C# · Universal Render Pipeline · Input System · Cinemachine 3 · ProBuilder

### Roadmap

- [ ] Documents and notes system (environmental storytelling)
- [ ] Locked doors and keys
- [ ] Player health, damage, ammo and reloading
- [ ] First robot enemy with a state machine (patrol, investigate, chase, attack)
- [ ] Building interiors, fog, lighting and sound
- [ ] Playable prototype on itch.io, full release on Steam

### Credits

Character model and animations: [Mixamo](https://www.mixamo.com).

---

## Português

**Português** · [English](#untitled-survival-horror-working-title)

Um survival horror em terceira pessoa em desenvolvimento na Unity 6, inspirado em *Resident Evil 2 Remake*, *Silent Hill* e na ficção científica clássica.

Num futuro em que a humanidade abandonou a Terra há muito tempo, robôs humanoides que nunca foram capazes de ferir ninguém começam a matar seus donos. Um detetive segue a trilha deles até uma cidade esquecida no planeta que todos deixaram para trás, onde exploração, recursos escassos e narrativa ambiental importam mais que poder de fogo.

> **Status:** protótipo inicial, em fase de greybox. O objetivo é acertar a sensação de andar, mirar e explorar antes de adicionar a arte.

### O que já está implementado

- **Câmera de ombro** (Cinemachine 3): desvio de obstáculos, zoom ao mirar com transição de FOV e alvo da câmera na altura do pescoço
- **Esconder o personagem quando a câmera chega perto**: o personagem some (mantendo a sombra) e reaparece com histerese, para não piscar
- **Movimento no estilo strafe**: o personagem olha para a direção da câmera, com velocidades diferentes para frente, para os lados e para trás
- **Sistema de animação**: dois blend trees 2D Freeform Directional (andar e mirar) controlados pela velocidade real do personagem em espaço local
- **Ferramenta de editor** que gera o greybox da primeira cidade (ruas, calçadas, prédios, bloqueios e muros de contorno) a partir de uma planta em escala
- **Inventário** com itens empilháveis baseado em ScriptableObjects
- **Sistema de interação** para pegar itens
- **Menu de pausa** e retícula de mira

### Tecnologias

Unity 6 (6000.5) · C# · Universal Render Pipeline · Input System · Cinemachine 3 · ProBuilder

### Próximos passos

- [ ] Sistema de documentos e notas (narrativa ambiental)
- [ ] Portas trancadas e chaves
- [ ] Vida, dano, munição e recarga do jogador
- [ ] Primeiro robô inimigo com máquina de estados (patrulhar, investigar, perseguir, atacar)
- [ ] Interiores dos prédios, neblina, iluminação e som
- [ ] Protótipo jogável no itch.io e lançamento completo na Steam

### Créditos

Modelo e animações do personagem: [Mixamo](https://www.mixamo.com).
