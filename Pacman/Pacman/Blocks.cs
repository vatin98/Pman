using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pacman
{
    public class Blocks
    {
        #region public vars
        public Rectangle rect;//прямоугольник в которую загоняется текстура  
        public int ID;//для определения нужной текстуры
        public Texture2D tex;
        #endregion

        #region Blocks
        public Blocks(Rectangle rect, Texture2D tex)//Конструктор
        {
            this.rect = rect;
            this.tex = tex;
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(tex, rect,Color.White);
        }
        #endregion
    }
}
