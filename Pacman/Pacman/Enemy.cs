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
    public class Enemy 
    {
        
        #region public vars
        public Texture2D spriteTex;//текстура героя
        public int ID;
        public Rectangle rect;//позиция спрайта
        public int speed = 2;//скорость персонажа 
        public int direction;//направление 
        
        #endregion

        #region private vars
        private Rectangle sourceRec = new Rectangle(0, 0, tileSize, tileSize);
        private Algorithm aStar;//класс основного алгоритма
        private Blocks[,] map;
        private int sleep = 0;//переменная для задержки одного фрейма в картинке при анимировании     
        private int frameCount = 0;
        private const int Speed = 4;
        private const int tileSize = 30;
        #endregion

        #region ctor
        public Enemy(Point point, Blocks[,] map)//конструктор принимающий как аргумент первоначальное положение объекта и карту
        {
            rect = new Rectangle(point.X, point.Y, tileSize, tileSize);
            aStar = new Algorithm(map);
            this.map = (Blocks[,])map.Clone();
        }
        #endregion 

        #region Loadcontent
        public void Loadcontent(ContentManager content, string texture)
        {
            spriteTex = content.Load<Texture2D>(texture);//Загрузка текстуры
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spritebatch)//анимирование персонажа
        {
            sleep++;
            if (frameCount == 2)
            {
                frameCount = 0;
            }
            sourceRec = new Rectangle(frameCount * tileSize + direction * 60, 0, tileSize, tileSize);
            spritebatch.Draw(spriteTex, rect, sourceRec, Color.White);
            if (sleep == 5)//если пробыл определенное время на одном фрейме
            {
                sleep = 0;
                frameCount++;//переход к следующему
            }
        }
        #endregion

        #region Movement
        public void Movement(int heroX, int heroY, int direct, bool fear)//движение призрака
        {
            //Инициализация координат пакмэна
            int x = heroX / tileSize;
            int y = heroY / tileSize;
            switch (ID)//получение ID для определения направления  движения призрака
            {
                case 0://pinky
                    if (!fear)
                    {
                        switch (direct)
                        {
                            case (int)EnumGhostMotion.up:
                                y -= Speed;
                             
                                break;
                            case (int)EnumGhostMotion.right:
                                x += Speed;

                                break;
                            case (int)EnumGhostMotion.down:
                                y += Speed;
                                break;
                            case (int)EnumGhostMotion.left:
                                x -= Speed;

                                break;
                        }
                    }
                    else
                    {
                        y = 19;
                        x = 25;
                    }
                    break;
                case 1://blinky цель пакман
                    if (!fear)
                    {
                        x = heroX / tileSize;
                        y = heroY / tileSize;
                    }
                    else
                    {
                        //коорд

                        y = 19;
                        x = 5;
                    }
                    break;
                case 2://clyde
                    if (!fear)
                    {
                        int[,] pathTMP = aStar.FindPath(rect.X / tileSize, rect.Y / tileSize, x, y);
                        if (pathTMP.GetLength(0) > 15)//если на расстоянии 15 клеток от пакмена
                        {
                            //получает первоначальные координаты пакмена
                            x = heroX / tileSize;
                            y = heroY / tileSize;
                        }
                        else
                        {
                            y = 19;
                            x = 3;
                        }
                    }
                    else
                    {
                        y = 19;
                        x = 27;
                    }
                    break;
                case 3://inky
                    if (!fear)
                    {
                        switch (direct)//направление
                        {
                            case 3:
                                x = (2 * heroX - rect.X) / tileSize;
                                y = (2 * heroY - 60 - rect.Y) / tileSize;
                                break;
                            case 0:
                                x = (2 * heroX + 60 - rect.X) / tileSize;
                                y = (2 * heroY - rect.Y) / tileSize;
                                break;
                            case 1:
                                x = (2 * heroX - rect.X) / tileSize;
                                y = (2 * heroY + 60 - rect.Y) / tileSize;
                                break;
                            case 2:
                                x = (2 * heroX - 60 - rect.X) / tileSize;
                                y = (2 * heroY - rect.Y) / tileSize;
                                break;
                        }
                    }
                    else
                    {
                        //коорд

                        y = 19;
                        x = 3;
                    }
                    break;
            }
            if (x < 0)//координата клетки, на которую можно наступить не может быть отрицательной
                x = 0;//значение при этом = 0
            if (x >= map.GetLength(1))//нельзя выйти за границу массива
                x = map.GetLength(1) - 1;//присваиваем при этом значение пустой ячейки
            if (y < 0)//аналогично x
                y = 0;
            if (y >= map.GetLength(0))//аналогично x
                y = map.GetLength(0) - 1;

            int[,] path = aStar.FindPath(rect.X / tileSize, rect.Y / tileSize, x, y);
              
            #region Collision
            //Обработка движения и коллизии
            if (path[0, 0] != -999)
            {
                if (rect.X < path[1, 0] * tileSize && map[rect.Y / tileSize, (rect.X + 27) / tileSize].ID != 1)
                    //обработка столкновения справа от персонажа
                {

                    if (map[(rect.Y + 27) / tileSize, (rect.X + 27) / tileSize].ID == 1 || map[(rect.Y + 27) /
                        tileSize, (rect.X) / tileSize].ID == 1) //в случае стен
                    {
                        rect.Y -= speed;// смещение назад

                    }
                    else
                    {
                        rect.X += speed;//движение дальше
                        direction = 1;
                    }
                }
                else
                {
                    if (rect.X > path[1, 0] * tileSize && map[rect.Y / tileSize, (rect.X - 3) / tileSize].ID != 1)
                        //обработка столкновения слева от персонажа
                    {

                        if (map[(rect.Y + 25) / tileSize, (rect.X + 25) / tileSize].ID == 1 || map[(rect.Y + 25) /
                            tileSize, (rect.X) / tileSize].ID == 1)
                        {
                            rect.Y -= speed;

                        }
                        else
                        {
                            rect.X -= speed;
                            direction = 3;
                        }
                    }
                    else
                    {
                        if (rect.Y < path[1, 1] * tileSize && map[(rect.Y + 27) / tileSize, rect.X / tileSize].ID != 1)//сверху
                        {
                            rect.Y += speed;
                            direction = 2;
                        }
                        else
                        {
                            if (rect.Y > path[1, 1] * tileSize && map[(rect.Y - 3) / tileSize, rect.X / tileSize].ID != 1)//снизу
                            {
                                rect.Y -= speed;
                                direction = 0;
                            }
                        }
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}

