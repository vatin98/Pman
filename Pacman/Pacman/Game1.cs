using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pacman
{
    public class Game1 : Game
    {

        #region public vars
        public int Widht;
        public int Height;
        #endregion

        #region private vars
        private const int tileSize = 30;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Menu menu = new Menu();//Упраление меню
        private int buttonState = 1;//Флажок для прорисовки меню

        private bool menuState = true;//Наличие меню
        private Texture2D blockTex;//Текстура блока
        private Texture2D bonusTex;//Текстура бонуса
        private Texture2D wayTex;//Черная свободная клетка
        private KeyboardState keyboardstate;//Нажатие клавиши
        private Blocks[,] mapBlocks;
        private List<Enemy> enemies = new List<Enemy>();

        private Point pointPac;//Начальное положение пакмэна
        private Hero heroPac;//класс героя
        private bool gameFlag = false;//флажок игрового процесса
        private bool gameOver = false;//конец игры
        private bool gameWin = false;//победа
        #endregion

        #region ctor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Widht = graphics.PreferredBackBufferWidth = 900;//Ширина экрана
            Height = graphics.PreferredBackBufferHeight = 630;//Высота экрана
            //graphics.IsFullScreen = true; 
            Content.RootDirectory = "Content";
            menu.ImageSize(ref graphics);
            IsMouseVisible = true;
            pointPac = new Point(90, 60);
            heroPac = new Hero(pointPac);

        }
        #endregion

        #region Initialize
        protected override void Initialize()
        {
            keyboardstate = Keyboard.GetState();
            base.Initialize();
        }
        #endregion

        #region NewGame
        public void NewGame(GameTime gameTime)
        {
            this.Initialize();
            CreateLevel();//1 уровень(карта)
                          //heroPac.Update(gameTime);

        }
        #endregion

        #region LoadContent
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            menu.LoadContent(this.Content);//Загрузка контента для меню
            blockTex = Content.Load<Texture2D>("wall");//загрузка текстуры для блока
            wayTex = Content.Load<Texture2D>("way");//Загрузка текстуры для бонуса
            bonusTex = Content.Load<Texture2D>("bonus");
            heroPac.Loadcontent(Content, "Pac");
        }
        #endregion

        #region Unload Content
        protected override void UnloadContent()
        {

        }
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            //Вся логика игры здесь 
            // Выход из игры...
            keyboardstate = Keyboard.GetState();

            #region menu
            if (keyboardstate.IsKeyDown(Keys.Escape))//возврат в меню
            {
                menuState = true;
                gameFlag = false;
            }
            if (menuState)//если меню
            {
                if (keyboardstate.IsKeyDown(Keys.Up))//если стрелка вверх
                {
                    menu.cursorState = 1;
                }
                else
                    if (keyboardstate.IsKeyDown(Keys.Down))//если стрелка вниз
                {
                    menu.cursorState = 2;
                }
            }
            if (keyboardstate.IsKeyDown(Keys.Enter) && menuState)
            {
                switch (menu.cursorState)
                {
                    case 1:
                        if (gameOver || gameWin)
                        {
                            enemies = new List<Enemy>();// лист призраков
                            CreateLevel();//карта
                            heroPac.rect.X = 90;//первоначальное положение пакмена
                            heroPac.rect.Y = 60;
                            heroPac.Score = 0;
                            gameOver = false;
                            gameWin = false;
                        }
                        menuState = false;
                        gameFlag = true;
                        NewGame(gameTime);

                        break;
                    case 2:
                        Exit();
                        break;
                }
            }
            #endregion

            if (gameFlag)//Если игра началась  (тут же обработка столкновений с блоками стены для пакмена)
            {
                PacManMotion();

                int heroX = (heroPac.rect.X + 15) / mapBlocks[0, 0].rect.Width;
                int heroY = (heroPac.rect.Y + 15) / mapBlocks[0, 0].rect.Height;
                if (mapBlocks[heroY, heroX].ID == (int)BlocksEnum.smallbonus)// прибавление бонусов 
                {
                    mapBlocks[heroY, heroX] = new Blocks(new Rectangle(heroX * tileSize, heroY * tileSize, tileSize, tileSize), wayTex);
                    heroPac.Score += 3;
                }
                if (mapBlocks[heroY, heroX].ID == (int)BlocksEnum.bigbouns)// прибавление бонусов 
                {
                    mapBlocks[heroY, heroX] = new Blocks(new Rectangle(heroX * tileSize, heroY * tileSize, tileSize, tileSize), wayTex);
                    heroPac.Score += 20;
                    heroPac.fear = true;
                }


                if (enemies.Count > 0)
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.Movement(heroPac.rect.X + 15, heroPac.rect.Y + 15, heroPac.direction, heroPac.fear);
                        if (heroPac.fear)
                        {
                            heroPac.fearCount++;
                        }
                        if (heroPac.fearCount > 1500)//если время действия таблетки прошло
                        {
                            heroPac.fear = false;
                            heroPac.fearCount = 0;
                        }
                    }

                }

                if (enemies.Count > 0)
                {
                    foreach (var enemy in enemies)
                    {
                        if (heroPac.rect.Intersects(enemy.rect))//если поймали 
                        {
                            gameFlag = false;
                            menuState = true;
                            gameOver = true;
                        }
                    }
                }
                gameWin = true;
                for (int i = 0; i < mapBlocks.GetLength(0); i++)
                {
                    for (int j = 0; j < mapBlocks.GetLength(1); j++)
                    {
                        if (mapBlocks[i, j].ID == 3)
                        {
                            gameWin = false;
                        }
                    }
                }


                if (gameOver)
                {
                    menu.GmOver(heroPac.Score);

                }
                if (gameWin)
                {
                    menu.GmWin(heroPac.Score);
                }



            }
            base.Update(gameTime);

        }
        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (menuState)
            {
                menu.Draw(spriteBatch, buttonState);
            }
            spriteBatch.End();
            spriteBatch.Begin();
            if (gameFlag)
            {

                GraphicsDevice.Clear(Color.Black);

                foreach (Blocks block in mapBlocks)
                {
                    block.Draw(spriteBatch);//Прорисовка блоков
                }
                heroPac.Draw(spriteBatch);

                if (enemies.Count > 0)
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.Draw(spriteBatch);//Прорисовка призраков
                    }
                }

            }
            spriteBatch.End();
            base.Draw(gameTime);

        }
        #endregion

        #region Pacman Motion
        private void PacManMotion()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right))//при движении вправо
            {
                if ((mapBlocks[(heroPac.rect.Y) / mapBlocks[0, 0].rect.Width, (heroPac.rect.X + tileSize) / 
                    mapBlocks[0, 0].rect.Height].ID != 1)
                    && (mapBlocks[(heroPac.rect.Y + 27) / mapBlocks[0, 0].rect.Width,
                    (heroPac.rect.X + tileSize) / mapBlocks[0, 0].rect.Height].ID != 1))
                {
                    heroPac.direction = 0;//1 выбор фрейма поворота вправо
                    heroPac.rect.X += heroPac.speed;//перемещение
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if ((mapBlocks[(heroPac.rect.Y) / mapBlocks[0, 0].rect.Width, (heroPac.rect.X - 1) / mapBlocks[0, 0].rect.Height].ID != 1)
                    && (mapBlocks[(heroPac.rect.Y + 27) / mapBlocks[0, 0].rect.Width,
                    (heroPac.rect.X - 1) / mapBlocks[0, 0].rect.Height].ID != 1))
                {
                    heroPac.direction = 2;//3  влево
                    heroPac.rect.X -= heroPac.speed;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if ((mapBlocks[(heroPac.rect.Y - 1) / mapBlocks[0, 0].rect.Width, (heroPac.rect.X) / mapBlocks[0, 0].rect.Height].ID != 1)
                    && (mapBlocks[(heroPac.rect.Y - 1) / mapBlocks[0, 0].rect.Width,
                    (heroPac.rect.X + 27) / mapBlocks[0, 0].rect.Height].ID != 1))
                {
                    heroPac.direction = 3;//0
                    heroPac.rect.Y -= heroPac.speed;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if ((mapBlocks[(heroPac.rect.Y + tileSize) / mapBlocks[0, 0].rect.Width, (heroPac.rect.X) /
                    mapBlocks[0, 0].rect.Height].ID != 1)
                    && (mapBlocks[(heroPac.rect.Y + tileSize) / mapBlocks[0, 0].rect.Width,
                    (heroPac.rect.X + 27) / mapBlocks[0, 0].rect.Height].ID != 1))
                {
                    heroPac.direction = 1;//2
                    heroPac.rect.Y += heroPac.speed;
                }
            }
        }
        #endregion

        #region Create Level
        public void CreateLevel()//создание уровня(карта, матрица)
        {

            mapBlocks = new Blocks[21, 30];
            string[] s = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "level1.txt");//прочитать матрицу из файла

            for (int i = 0; i < s.Length; i++)
            {
                for (int j = 0; j < s[i].Length; j++)
                {
                    Rectangle rectan = new Rectangle(j * tileSize, i * tileSize, tileSize, tileSize);
                    Blocks block;
                    switch (s[i][j])
                    {
                        case '0'://в случае 0 прибавление бонуса в основную карту 
                            rectan = new Rectangle(j * tileSize + 10, i * tileSize + 10, 10, 10);
                            block = new Blocks(rectan, bonusTex);
                            block.ID = (int)BlocksEnum.smallbonus;
                            break;
                        case '4'://в случае 4 прибавление квадрата пути в основную карту
                            block = new Blocks(rectan, wayTex);
                            block.ID = (int)BlocksEnum.road;
                            break;
                        case 'x':
                            block = new Blocks(rectan, blockTex);//в случае x прибавление блока стены в основную карту
                            block.ID = (int)BlocksEnum.wall;
                            break;
                        case '5':
                            block = new Blocks(rectan, bonusTex);//добавление таблетки
                            block.ID = (int)BlocksEnum.bigbouns;
                            break;
                        default:
                            block = new Blocks(rectan, blockTex);
                            block.ID = (int)BlocksEnum.wall;
                            break;
                    }
                    mapBlocks[i, j] = block; //заполнение кубиками                          
                }
            }
            Enemy pinky = new Enemy(new Point(480, 300), mapBlocks);//положение пинки, передача карты
            pinky.Loadcontent(Content, "pinky");
            pinky.ID = 0;//ID для обращения к нему
            enemies.Add(pinky);//добавление призрака
            Enemy blinky = new Enemy(new Point(450, 330), mapBlocks);//положение блинки, передача карты
            blinky.Loadcontent(Content, "blinky");
            blinky.ID = 1;
            enemies.Add(blinky);//добавление призрака
            Enemy clyde = new Enemy(new Point(420, 300), mapBlocks);//положение клайда, передача карты
            clyde.Loadcontent(Content, "clyde");
            clyde.ID = 2;
            enemies.Add(clyde);//добавление призрака
            Enemy inky = new Enemy(new Point(390, 330), mapBlocks);//положение инки, передача карты
            inky.Loadcontent(Content, "inky");
            inky.ID = 3;
            enemies.Add(inky);//добавление призрака 


        }
        #endregion
    }

}