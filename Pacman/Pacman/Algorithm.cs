using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman
{
    class Algorithm 
    {
        #region private vars
        private int[,] map;
        private int height;//Высота экрана
        private int width;//Ширина
        #endregion

        #region Algorithm 
        public Algorithm(Blocks[,] arr)//Передача карты в метод
        {
            map = new int[arr.GetLength(0), arr.GetLength(1)];
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    switch (arr[i, j].ID)//Смена существующего массива
                    {
                        case 1:
                            map[i, j] = -2;//На -2 в случае стены
                            break;
                        default:
                            map[i, j] = -1;//на -1 в случае свободного пути
                            break;
                    }
                }
            }
            height = arr.GetLength(0);
            width = arr.GetLength(1);
        }
        #endregion

        #region FindPath
        public int[,] FindPath(int startX, int startY, int targetX, int targetY)//Ищем путь(принимает координаты цели и самого врага начальные)
        {
            bool flag = true;
            int[,] tempMap = (int[,])map.Clone();//Перезапись массива, чтобы не потерять значения
            tempMap[startY, startX] = 0;//Начальное положение врага
            int step = 0;
            while (flag)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (tempMap[y, x] == step)//Перебор массива, если совпадает с шагом
                        {
                            //Ставим значение шага+1 в соседние ячейки (если они проходимы)
                            if (y - 1 >= 0 && tempMap[y - 1, x] == -1)//смотрит наверх, если в ячейке -1
                                tempMap[y - 1, x] = step + 1;//заменяем значени на +1
                            if (x - 1 >= 0 && tempMap[y, x - 1] == -1)//вправо
                                tempMap[y, x - 1] = step + 1;

                            if (y + 1 < height && tempMap[y + 1, x] == -1)//вниз
                                tempMap[y + 1, x] = step + 1;
                            if (x + 1 < width && tempMap[y, x + 1] == -1)//влево
                                tempMap[y, x + 1] = step + 1;
                        }
                    }
                }
                step++;
                if (tempMap[targetY, targetX] != -1)//доходим до того момента, когда ячейка цели не равна -1
                    flag = false;
                if (step > height * width)
                {
                    int[,] error = new int[1, 1];
                    error[0, 0] = -999;//
                    return error;
                }


            }
          
            int[,] res = new int[step + 1, 2];//результирующий массив для записи значений 
            //Начальные координаты призрака
            res[0, 0] = startX;
            res[0, 1] = startY;
            //Окончательные координаты данной таблицы - координаты Цели
            res[res.GetLength(0) - 1, 0] = targetX;
            res[res.GetLength(0) - 1, 1] = targetY;

            FindTheWave(step, tempMap, res, targetX, targetY);// для повторного поиска кратчайшего пути в зависимости от перемещения пакмена
            return res;
        }
        #endregion

        #region FindTheWave
        void FindTheWave(int step, int[,] Map, int[,] res, int x, int y)//Метод нахождения волны , прокладываем маршрут от конечной точки до начального  положения
        {
            if (step >= 0)//если не в начальной позиции, проверка клетки
            {
                if (y >= 1 && Map[y - 1, x] == step - 1)//проверка значения на клетку меньше от полученной позиции и сравнение с шагом на 1 меньше
                {
                    //Результирующий массив заполняется при совпадении
                    res[step - 1, 0] = x;
                    res[step - 1, 1] = y - 1;
                    FindTheWave(step - 1, Map, res, x, y - 1);//Рекурсия(вызов этого же метода), просмотр позиции уже с полученной точки
                }
                //Те же условия
                if (x >= 1 && Map[y, x - 1] == step - 1)
                {
                    res[step - 1, 0] = x - 1;
                    res[step - 1, 1] = y;
                    FindTheWave(step - 1, Map, res, x - 1, y);
                }
                //Условия чтобы не выйти за границу экрана
                if (y + 1 < height && Map[y + 1, x] == step - 1)
                {
                    res[step - 1, 0] = x;
                    res[step - 1, 1] = y + 1;
                    FindTheWave(step - 1, Map, res, x, y + 1);
                }

                if (x + 1 < width && Map[y, x + 1] == step - 1)
                {
                    res[step - 1, 0] = x + 1;
                    res[step - 1, 1] = y;
                    FindTheWave(step - 1, Map, res, x + 1, y);
                }
            }
        }
        #endregion

    }
}
