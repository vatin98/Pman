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
    public class Menu
    {
        #region private vars
        private Texture2D texfon;//Фон
        private Texture2D pacman;
        private Texture2D buttonGame;//начать игру
        private Texture2D buttonExit;//Завершить
        private Texture2D lineMenu;
        private Texture2D GameOver;
        private Texture2D WinGame;
        private Texture2D Num;
        private Texture2D ScoreTex;
        private Vector2 menuPos, buttonGamePos, buttonExitPos, pacmanPos;
        private Rectangle RectWindFon; //область экрана для рисования спрайта 
        private Rectangle RectImgFon1; //область рисования текстуры play
        private Rectangle RectImgFon2; //область рисования текстуры pacman
        private Rectangle RectImgFon3; //область рисования текстуры Exit
        private Rectangle RectImgLineMenu; //для отрисовки выбора в меню
        private Rectangle ScoreRec;
        private Rectangle NumS;
        private List<Rectangle> ScoreRects;
        private bool scoreFlag = false;
        private int count = 0;
        #endregion

        #region public vars
        public int cursorState = 1;
        #endregion

        #region Ctor
        public Menu()//начальное расположение текстур
        {
            menuPos = new Vector2(0, 0);
            pacmanPos = new Vector2(0, 0);
            buttonGamePos = new Vector2(0, 0);
            buttonExitPos = new Vector2(0, 0);
        }
        #endregion
         
        #region Image Size
        public void ImageSize(ref GraphicsDeviceManager graphics)
        {
            RectWindFon = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);//Координаты фона
            RectImgFon1 = new Rectangle(10, 150, 300, 50);//координаты выхода старта
            RectImgFon2 = new Rectangle(50, 20, 180, 80);//координаты названия Pacman
            RectImgFon3 = new Rectangle(30, 300, 140, 40);//координаты кнопки выхода
            RectImgLineMenu = new Rectangle(10, 205, 300, 6);//это линия выбора в меню
            ScoreRec = new Rectangle(100, 500, 209, 39);
        }
        #endregion

        #region LoadContent
        public void LoadContent(ContentManager content)
        {
            texfon = content.Load<Texture2D>("fon");
            pacman = content.Load<Texture2D>("pacman");
            buttonGame = content.Load<Texture2D>("start");
            buttonExit = content.Load<Texture2D>("Exit");
            lineMenu = content.Load<Texture2D>("LineMenu");
            GameOver = content.Load<Texture2D>("GameOver");
            WinGame = content.Load<Texture2D>("YouWin");
            Num = content.Load<Texture2D>("Font");
            ScoreTex = content.Load<Texture2D>("Score");
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch, int ButtonState)
        {

            spriteBatch.Draw(texfon, RectWindFon, null, Color.White);
            while (ButtonState != 4)
            {
                switch (ButtonState)
                {
                    case 1:
                        spriteBatch.Draw(buttonGame, RectImgFon1, null, Color.White);
                        break;
                    case 2:
                        spriteBatch.Draw(buttonExit, RectImgFon3, null, Color.White);
                        break;
                    case 3:
                        spriteBatch.Draw(pacman, RectImgFon2, null, Color.White);
                        break;
                }
                ButtonState++;
            }
            if (cursorState == 1)
            {
                RectImgLineMenu.Y = 205;
                RectImgLineMenu.X = 10;
                RectImgLineMenu.Width = 300;
                spriteBatch.Draw(lineMenu, RectImgLineMenu, null, Color.White);
            }
            else
            {
                RectImgLineMenu.Y = 345;
                RectImgLineMenu.X = 30;
                RectImgLineMenu.Width = 140;
                spriteBatch.Draw(lineMenu, RectImgLineMenu, null, Color.White);
            }
            if (scoreFlag)
            {
                spriteBatch.Draw(ScoreTex, ScoreRec, Color.White);
                for (int i = 0; i < count; i++)
                {
                    NumS = new Rectangle(320 + 100 * i + 5 * i, 500, 100, 120);
                    spriteBatch.Draw(Num, NumS, ScoreRects[i], Color.White);
                }

            }

        }
        #endregion

        #region gameover\win\score
        public void GmOver(long score)
        {
            texfon = GameOver;
            scoreFlag = true;
            printScore(score);
        }
        public void GmWin(long score)
        {
            texfon = WinGame;
            scoreFlag = true;
            printScore(score);
        }

        public void printScore(long Score)
        {
            ScoreRects = new List<Rectangle>();
            string s = Score.ToString();
            count = s.Length;
            for (int i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case '0':
                        ScoreRects.Add(new Rectangle(553, 220, 100, 120));
                        break;
                    case '1':
                        ScoreRects.Add(new Rectangle(10, 30, 100, 120));
                        break;
                    case '2':
                        ScoreRects.Add(new Rectangle(144, 30, 100, 120));
                        break;
                    case '3':
                        ScoreRects.Add(new Rectangle(275, 30, 100, 120));
                        break;
                    case '4':
                        ScoreRects.Add(new Rectangle(410, 30, 100, 120));
                        break;
                    case '5':
                        ScoreRects.Add(new Rectangle(550, 30, 100, 120));
                        break;
                    case '6':
                        ScoreRects.Add(new Rectangle(682, 30, 100, 120));
                        break;
                    case '7':
                        ScoreRects.Add(new Rectangle(126, 220, 100, 120));
                        break;
                    case '8':
                        ScoreRects.Add(new Rectangle(261, 220, 100, 120));
                        break;
                    case '9':
                        ScoreRects.Add(new Rectangle(407, 220, 100, 120));
                        break;
                }
            }
        }
        #endregion
    }
}
