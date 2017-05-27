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
    public class Hero
    {
        
        #region public vars
        public Texture2D spriteTex;//текстура героя
        public Rectangle rect;//позиция спрайта
        public long Score = 0;//счет
        public int speed = 2;//скорость персонажа 
        public int direction;//направление
        public bool fear = false;
        public int fearCount = 0;
        #endregion

        #region private vars
        private Rectangle sourceRec;
        private int frameCount = 0;
        private int sleep = 0;
        private const int tileSize = 30;
        #endregion

        #region Ctor
        public Hero(Point point)//конструктор принимающий как аргумент первоначальное положение объекта
        {
            rect = new Rectangle(point.X, point.Y, tileSize, tileSize);
        }
        #endregion

        #region Loadcontent
        public void Loadcontent(ContentManager content, string texture)
        {
            spriteTex = content.Load<Texture2D>(texture);
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spritebatch)//передвижение по картинке для создания анимации
        {
            sleep++;//для остановки на одном фрейме с целью снижения смены фреймов в одном спрайте
            if (frameCount == 2)
            {
                frameCount = 0;
            }
            sourceRec = new Rectangle(frameCount * tileSize, direction * tileSize, tileSize, tileSize);
            spritebatch.Draw(spriteTex, rect, sourceRec, Color.White);
            if (sleep == 5)//если пробыл определенное время на одном фрейме
            {
                sleep = 0;//Обнуление счетчика
                frameCount++;//смена фрейма
            }
        }
        #endregion
    }
}
