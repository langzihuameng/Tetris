using System;
using System.Threading;

namespace FowerDreams
{
    class Program
    {
        static void Main()
        {
            WelcomeMyFriend.SayWelcome();       // 欢迎界面
            WelcomeMyFriend.SayExplain();       // 游戏说明

            // 引用实例
            var tetrisPrimary = new TetrisPrimary(new InitTetrisSize(), new Diamonds());

            // 制作定时器（下落）
            Timer timer = new Timer(new TimerCallback((object state) => { tetrisPrimary.drawMapForOutput.UpdateDrop(); }));
            timer.Change(0, Diamonds.speed);

            // 绑定事件处理器 ( 左、右、下 )
            tetrisPrimary.Left += tetrisPrimary.LeftMove;
            tetrisPrimary.Right += tetrisPrimary.RightMove;
            tetrisPrimary.Down += tetrisPrimary.DownMove;
            tetrisPrimary.Up += tetrisPrimary.UpChange;

            while (true)
            {
                tetrisPrimary.drawMapForOutput.DrawMap();       // 画地图   

                if (Console.KeyAvailable)           // 判断是否从键盘按下了键
                {
                    var inputKey = Console.ReadKey().Key;
                    tetrisPrimary.MoveDiamonds(inputKey);
                }
            }
        }
    }


    class TetrisPrimary
    {
        public int Speed { get; set; }

        public static int Flag = 0;     // 标记方块转到了哪种形式

        public DrawMapForOutput drawMapForOutput;   // 画地图

        private IMapSizeAndArray _mapSizeAndArray;  // 大小与地图
        private IMapSizeAndArray _mapSizeAndDiamonds;   // 方块

        public event EventHandler Left;
        public event EventHandler Right;
        public event EventHandler Down;
        public event EventHandler Up;

        // 整体框架 和 方块
        public TetrisPrimary(IMapSizeAndArray mapSizeAndArray, IMapSizeAndArray mapSizeAndDiamonds)
        {
            _mapSizeAndArray = mapSizeAndArray;
            _mapSizeAndDiamonds = mapSizeAndDiamonds;

            drawMapForOutput = new DrawMapForOutput(mapSizeAndArray, mapSizeAndDiamonds);
        }

        internal void MoveDiamonds(ConsoleKey inputKey)
        {
            switch (inputKey)
            {
                case ConsoleKey.LeftArrow:
                    Left?.Invoke(this, null);
                    break;
                case ConsoleKey.RightArrow:
                    Right?.Invoke(this, null);
                    break;
                case ConsoleKey.UpArrow:
                    Up?.Invoke(this, null);
                    break;
                case ConsoleKey.DownArrow:
                    Down?.Invoke(this, null);
                    break;
            }
        }

        internal void LeftMove(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                var x = DrawMapForOutput.flagPos[i, 0];
                var y = DrawMapForOutput.flagPos[i, 1];

                if (_mapSizeAndArray._array[x, y - 1] == 2)
                {
                    return;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                var x = DrawMapForOutput.flagPos[i, 0];
                var y = DrawMapForOutput.flagPos[i, 1];

                _mapSizeAndArray._array[x, y] = 0;
                _mapSizeAndArray._array[x, y - 1] = 1;

                --DrawMapForOutput.flagPos[i, 1];
            }
        }

        internal void RightMove(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                var x = DrawMapForOutput.flagPos[i, 0];
                var y = DrawMapForOutput.flagPos[i, 1];

                if (_mapSizeAndArray._array[x, y + 1] == 2)
                {
                    return;
                }
            }

            for (int i = 3; i >= 0; i--)
            {
                var x = DrawMapForOutput.flagPos[i, 0];
                var y = DrawMapForOutput.flagPos[i, 1];

                _mapSizeAndArray._array[x, y] = 0;
                _mapSizeAndArray._array[x, y + 1] = 1;

                ++DrawMapForOutput.flagPos[i, 1];
            }
        }

        internal void DownMove(object sender, EventArgs e)
        {
            drawMapForOutput.UpdateDrop();
        }

        internal void UpChange(object sender, EventArgs e)
        {
            Change(((Diamonds)_mapSizeAndDiamonds).SevenStyle);
        }

        private void Change(int n)
        {
            switch (n)
            {
                case 1:     // 第一种方块
                    switch (Flag % 4)
                    {
                        case 0:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x, y - 1] == 0 && _mapSizeAndArray._array[x - 1, y - 1] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y - 1] =
                                        _mapSizeAndArray._array[x + 1, y] = _mapSizeAndArray._array[x + 1, y + 1] = 0;

                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] -= 1; DrawMapForOutput.flagPos[0, 1] -= 1;
                                    DrawMapForOutput.flagPos[1, 0] -= 1;
                                    DrawMapForOutput.flagPos[2, 0] -= 1;
                                    DrawMapForOutput.flagPos[3, 1] -= 2;

                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 1, y + 1] = _mapSizeAndArray._array[x + 2, y] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 1:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x, y + 1] == 0 && _mapSizeAndArray._array[x, y + 2] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 1, y + 1] = _mapSizeAndArray._array[x + 2, y] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[1, 0] -= 1; DrawMapForOutput.flagPos[1, 1] += 1;
                                    DrawMapForOutput.flagPos[2, 0] -= 1; DrawMapForOutput.flagPos[2, 1] += 1;
                                    DrawMapForOutput.flagPos[3, 0] -= 1; DrawMapForOutput.flagPos[3, 1] += 1;

                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x, y + 2] = _mapSizeAndArray._array[x + 1, y + 1] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }


                            break;

                        case 2:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x + 1, y + 2] == 0 && _mapSizeAndArray._array[x + 2, y + 2] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x, y + 2] = _mapSizeAndArray._array[x + 1, y + 1] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 1] += 2;
                                    DrawMapForOutput.flagPos[1, 0] += 1;
                                    DrawMapForOutput.flagPos[2, 0] += 1;
                                    DrawMapForOutput.flagPos[3, 0] += 1; DrawMapForOutput.flagPos[3, 1] += 1;

                                    x = DrawMapForOutput.flagPos[0, 0];         // 获取最新的第一个小方格 
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y - 1] =
                                        _mapSizeAndArray._array[x + 1, y] = _mapSizeAndArray._array[x + 2, y] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 3:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x + 2, y - 2] == 0 && _mapSizeAndArray._array[x + 2, y - 1] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y - 1] =
                                        _mapSizeAndArray._array[x + 1, y] = _mapSizeAndArray._array[x + 2, y] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] += 1; DrawMapForOutput.flagPos[0, 1] -= 1;
                                    DrawMapForOutput.flagPos[1, 0] += 1; DrawMapForOutput.flagPos[1, 1] -= 1;
                                    DrawMapForOutput.flagPos[2, 0] += 1; DrawMapForOutput.flagPos[2, 1] -= 1;

                                    x = DrawMapForOutput.flagPos[0, 0];         // 获取最新的第一个小方格 
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y - 1] =
                                        _mapSizeAndArray._array[x + 1, y] = _mapSizeAndArray._array[x + 1, y + 1] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;
                    }
                    break;

                case 2:
                    switch (Flag % 4)
                    {
                        case 0:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x - 1, y] == 0 && _mapSizeAndArray._array[x - 1, y + 1] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 1, y + 1] = _mapSizeAndArray._array[x + 1, y + 2] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] -= 1;
                                    DrawMapForOutput.flagPos[1, 0] -= 2; DrawMapForOutput.flagPos[1, 1] += 1;
                                    DrawMapForOutput.flagPos[2, 0] -= 1; DrawMapForOutput.flagPos[2, 1] -= 1;
                                    DrawMapForOutput.flagPos[3, 1] -= 2;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x + 1, y] = _mapSizeAndArray._array[x + 2, y] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 1:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x + 1, y + 1] == 0 && _mapSizeAndArray._array[x + 1, y + 2] == 0
                                     && _mapSizeAndArray._array[x + 2, y + 2] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x + 1, y] = _mapSizeAndArray._array[x + 2, y] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] += 1;
                                    DrawMapForOutput.flagPos[1, 0] += 1;
                                    DrawMapForOutput.flagPos[2, 1] += 2;
                                    DrawMapForOutput.flagPos[3, 1] += 2;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x, y + 2] = _mapSizeAndArray._array[x + 1, y + 2] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 2:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x + 1, y + 1] == 0 && _mapSizeAndArray._array[x - 1, y + 2] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x, y + 2] = _mapSizeAndArray._array[x + 1, y + 2] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] -= 1; DrawMapForOutput.flagPos[0, 1] += 2;
                                    DrawMapForOutput.flagPos[1, 1] += 1;
                                    DrawMapForOutput.flagPos[2, 0] += 1; DrawMapForOutput.flagPos[2, 1] -= 1;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 2, y - 1] = _mapSizeAndArray._array[x + 2, y] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 3:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x + 1, y - 2] == 0 && _mapSizeAndArray._array[x + 2, y - 2] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 2, y] = _mapSizeAndArray._array[x + 2, y - 1] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] += 1; DrawMapForOutput.flagPos[0, 1] -= 2;
                                    DrawMapForOutput.flagPos[1, 0] += 1; DrawMapForOutput.flagPos[1, 1] -= 2;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 1, y + 1] = _mapSizeAndArray._array[x + 1, y + 2] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;
                    }
                    break;

                case 3:
                    switch (Flag % 4)
                    {
                        case 0:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x, y - 1] == 0 && _mapSizeAndArray._array[x - 1, y - 1] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y - 2] =
                                        _mapSizeAndArray._array[x + 1, y - 1] = _mapSizeAndArray._array[x + 1, y] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] -= 1; DrawMapForOutput.flagPos[0, 1] -= 1;
                                    DrawMapForOutput.flagPos[1, 0] -= 1; DrawMapForOutput.flagPos[1, 1] += 1;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 2, y] = _mapSizeAndArray._array[x + 2, y + 1] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 1:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x + 1, y + 1] == 0 && _mapSizeAndArray._array[x + 1, y + 2] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 2, y] = _mapSizeAndArray._array[x + 2, y + 1] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] += 1;
                                    DrawMapForOutput.flagPos[1, 1] += 1;
                                    DrawMapForOutput.flagPos[2, 0] -= 1; DrawMapForOutput.flagPos[2, 1] += 2;
                                    DrawMapForOutput.flagPos[3, 1] -= 1;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x, y + 2] = _mapSizeAndArray._array[x + 1, y] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 2:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x - 1, y] == 0 && _mapSizeAndArray._array[x + 1, y + 1] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x, y + 2] = _mapSizeAndArray._array[x + 1, y] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] -= 1;
                                    DrawMapForOutput.flagPos[1, 0] -= 1;
                                    DrawMapForOutput.flagPos[2, 1] -= 1;
                                    DrawMapForOutput.flagPos[3, 1] += 1;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x + 1, y + 1] = _mapSizeAndArray._array[x + 2, y + 1] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 3:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x + 2, y - 1] == 0 && _mapSizeAndArray._array[x + 2, y] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x + 1, y + 1] = _mapSizeAndArray._array[x + 2, y + 1] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] += 1; DrawMapForOutput.flagPos[0, 1] += 1;
                                    DrawMapForOutput.flagPos[1, 0] += 2; DrawMapForOutput.flagPos[1, 1] -= 2;
                                    DrawMapForOutput.flagPos[2, 0] += 1; DrawMapForOutput.flagPos[2, 1] -= 1;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y - 2] =
                                        _mapSizeAndArray._array[x + 1, y - 1] = _mapSizeAndArray._array[x + 1, y] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;
                    }
                    break;

                case 4:
                    switch (Flag % 2)
                    {
                        case 0:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x - 1, y + 2] == 0 && _mapSizeAndArray._array[x, y + 2] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x + 1, y + 1] = _mapSizeAndArray._array[x + 1, y + 2] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] -= 1; DrawMapForOutput.flagPos[0, 1] += 2;
                                    DrawMapForOutput.flagPos[2, 0] -= 1; DrawMapForOutput.flagPos[2, 1] += 1;
                                    DrawMapForOutput.flagPos[3, 1] -= 1;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y - 1] =
                                        _mapSizeAndArray._array[x + 1, y] = _mapSizeAndArray._array[x + 2, y - 1] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 1:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x + 1, y - 2] == 0 && _mapSizeAndArray._array[x + 2, y] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y - 1] =
                                        _mapSizeAndArray._array[x + 1, y] = _mapSizeAndArray._array[x + 2, y - 1] = 0;



                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] += 1; DrawMapForOutput.flagPos[0, 1] -= 2;
                                    DrawMapForOutput.flagPos[2, 0] += 1; DrawMapForOutput.flagPos[2, 1] -= 1;
                                    DrawMapForOutput.flagPos[3, 1] += 1;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x + 1, y + 1] = _mapSizeAndArray._array[x + 1, y + 2] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;
                    }
                    break;

                case 5:
                    switch (Flag % 2)
                    {
                        case 0:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x - 1, y - 1] == 0 && _mapSizeAndArray._array[x, y - 1] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x + 1, y - 1] = _mapSizeAndArray._array[x + 1, y] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] -= 1; DrawMapForOutput.flagPos[0, 1] -= 1;
                                    DrawMapForOutput.flagPos[1, 1] -= 2;
                                    DrawMapForOutput.flagPos[2, 0] -= 1; DrawMapForOutput.flagPos[2, 1] += 1;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 1, y + 1] = _mapSizeAndArray._array[x + 2, y + 1] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 1:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];
                                if (_mapSizeAndArray._array[x + 2, y] == 0 && _mapSizeAndArray._array[x + 1, y + 2] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 1, y + 1] = _mapSizeAndArray._array[x + 2, y + 1] = 0;



                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] += 1; DrawMapForOutput.flagPos[0, 1] += 1;
                                    DrawMapForOutput.flagPos[1, 1] += 2;
                                    DrawMapForOutput.flagPos[2, 0] += 1; DrawMapForOutput.flagPos[2, 1] -= 1;


                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x + 1, y - 1] = _mapSizeAndArray._array[x + 1, y] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;
                    }
                    break;

                case 8:
                case 9:
                    switch (Flag % 2)
                    {
                        case 0:
                            try         // 防止数组越界 
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];

                                if (_mapSizeAndArray._array[x - 1, y] == 0 && _mapSizeAndArray._array[x - 2, y] == 0 &&
                                         _mapSizeAndArray._array[x - 3, y] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x, y + 2] = _mapSizeAndArray._array[x, y + 3] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] -= 3;
                                    DrawMapForOutput.flagPos[1, 0] -= 2; DrawMapForOutput.flagPos[1, 1] -= 1;
                                    DrawMapForOutput.flagPos[2, 0] -= 1; DrawMapForOutput.flagPos[2, 1] -= 2;
                                    DrawMapForOutput.flagPos[3, 1] -= 3;

                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 2, y] = _mapSizeAndArray._array[x + 3, y] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;

                        case 1:
                            try
                            {
                                var x = DrawMapForOutput.flagPos[0, 0];
                                var y = DrawMapForOutput.flagPos[0, 1];

                                if (_mapSizeAndArray._array[x + 3, y + 1] == 0 && _mapSizeAndArray._array[x + 3, y + 2] == 0 &&
                                         _mapSizeAndArray._array[x + 3, y + 3] == 0)
                                {
                                    // 先将所有方块置 0
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x + 1, y] =
                                        _mapSizeAndArray._array[x + 2, y] = _mapSizeAndArray._array[x + 3, y] = 0;


                                    // 改变方块的下标
                                    DrawMapForOutput.flagPos[0, 0] += 3;
                                    DrawMapForOutput.flagPos[1, 0] += 2; DrawMapForOutput.flagPos[1, 1] += 1;
                                    DrawMapForOutput.flagPos[2, 0] += 1; DrawMapForOutput.flagPos[2, 1] += 2;
                                    DrawMapForOutput.flagPos[3, 1] += 3;

                                    x = DrawMapForOutput.flagPos[0, 0];
                                    y = DrawMapForOutput.flagPos[0, 1];

                                    // 设置方块下标对应数组中的元素的值
                                    _mapSizeAndArray._array[x, y] = _mapSizeAndArray._array[x, y + 1] =
                                        _mapSizeAndArray._array[x, y + 2] = _mapSizeAndArray._array[x, y + 3] = 1;

                                    ++Flag;     // 标记第一种已经变过形了
                                }
                                else
                                    return;
                            }
                            catch
                            {
                                return;
                            }

                            break;
                    }
                    break;
            }
        }
    }


    // 包含数组与大小的接口
    interface IMapSizeAndArray          // 解决了类之间的紧耦合
    {
        int Row { get; set; }
        int Col { get; set; }
        int[,] _array { get; set; }

        int GetRow();
        int GetCol();
    }


    class DrawMapForOutput
    {
        private IMapSizeAndArray _mapSizeAndArray;      // 引用接口 (整体框架）
        private IMapSizeAndArray _mapSizeAndDiamonds;   // 方块
        public static int[,] flagPos = new int[4, 2];          // 用于标记方块的位置

        public static int RandomNum;        // 随机数记录

        public DrawMapForOutput(IMapSizeAndArray mapSizeAndArray, IMapSizeAndArray mapSizeAndDiamonds)
        {
            Console.CursorVisible = false;              //隐藏光标 

            _mapSizeAndArray = mapSizeAndArray;
            _mapSizeAndDiamonds = mapSizeAndDiamonds;

            // 将方块赋值给整体框架
            InitDiamonds();

            for (int i = 0; i < _mapSizeAndArray.Col; i++)          // 将边界设置为 2
            {
                _mapSizeAndArray._array[_mapSizeAndArray.Row - 1, i] = 2;
            }
            for (int i = 0; i < _mapSizeAndArray.Row; i++)
            {
                _mapSizeAndArray._array[i, 0] = 2;
                _mapSizeAndArray._array[i, _mapSizeAndArray.Col - 1] = 2;
            }
        }

        public void DrawMap()
        {
            const int x = 30;       // 地图较控制台的初始位置
            int y = 5;

            for (int i = 0; i < _mapSizeAndArray.Row; i++)
            {
                Console.SetCursorPosition(x, y);    // 设置光标

                for (int j = 0; j < _mapSizeAndArray.Col; j++)
                {
                    if (j == 0 || j == _mapSizeAndArray.Col - 1 || i == _mapSizeAndArray.Row - 1 ||
                        _mapSizeAndArray._array[i, j] == 1 || _mapSizeAndArray._array[i, j] == 2)
                    {
                        Console.Write("■");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }

                Console.WriteLine();
                y += 1;
            }
        }

        //  初始化方块的位置
        private void InitDiamonds()
        {
            int k = 0;

            for (int i = 0; i < _mapSizeAndDiamonds.Row; i++)
            {
                for (int j = 0; j < _mapSizeAndDiamonds.Col; j++)
                {
                    _mapSizeAndArray._array[i + 1, j + 5] = _mapSizeAndDiamonds._array[i, j];

                    if (_mapSizeAndDiamonds._array[i, j] == 1)
                    {
                        flagPos[k, 0] = i + 1;
                        flagPos[k, 1] = j + 5;
                        ++k;
                    }
                }
            }

        }

        public void UpdateDrop()        // 下落更新
        {
            for (int i = 0; i < 4; i++)
            {
                if (_mapSizeAndArray._array[flagPos[i, 0] + 1, flagPos[i, 1]] == 2)
                {
                    for (int j = 3; j >= 0; j--)
                        _mapSizeAndArray._array[flagPos[j, 0], flagPos[j, 1]] = 2;

                    RemoveDiamonds();


                    ((Diamonds)_mapSizeAndDiamonds).SetArrayEmpty();        // 将 方块数组的元素都清空

                    ((Diamonds)_mapSizeAndDiamonds).SetDiamonds(RandomNum);         // 获取新的方块
                    InitDiamonds();

                    TetrisPrimary.Flag = 0;         // 这句代码我会铭记于心 （折磨我 ！！！！！！！！！！！！！)

                    return;
                }
            }

            for (int i = 3; i >= 0; i--)
            {
                _mapSizeAndArray._array[flagPos[i, 0], flagPos[i, 1]] = 0;
                _mapSizeAndArray._array[flagPos[i, 0] + 1, flagPos[i, 1]] = 1;
                ++flagPos[i, 0];
            }
        }

        private bool JudgeLosing()
        {
            return flagPos[3, 0] < 5;
        }


        private void RemoveDiamonds()
        {
            int min = flagPos[0, 0];        // 找到判断方块消除的范围
            int max = flagPos[3, 0];
            int flag = 0;   // 标记一行是否都是方块

            for (int i = max; i >= min; i--)
            {
                flag = 0;
                for (int j = 1; j < _mapSizeAndArray.Col - 1; j++)
                {
                    if (_mapSizeAndArray._array[i, j] != 2)
                    {
                        flag = 1;
                        break;
                    }
                }

                if (flag == 0)
                {
                    for (int k = i; k > 0; k--)
                    {
                        for (int l = 1; l < _mapSizeAndArray.Col - 1; l++)
                        {
                            _mapSizeAndArray._array[k, l] = _mapSizeAndArray._array[k - 1, l];
                        }
                    }
                    ++i;
                }
            }
        }

    }


    class InitTetrisSize : IMapSizeAndArray
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int[,] _array { get; set; }


        public InitTetrisSize()
        {
            Console.Title = "作者：Flower Dreams";

            Row = 21;   //  20行 10列
            Col = 12;
            _array = new int[Row, Col];   // 引用实例
        }

        public int GetRow()
        {
            return Row;
        }

        public int GetCol()
        {
            return Col;
        }
    }


    class Diamonds : IMapSizeAndArray           // 方块的大小类
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int[,] _array { get; set; }
        public int SevenStyle { get; set; }     // 七种不同的方块标记

        public static int speed;

        public Diamonds()
        {
            Row = 2;
            Col = 4;
            _array = new int[Row, Col];         // 四行四列的数组用于存放方块 

            SetDiamonds(1);
        }


        public int GetCol()
        {
            return Col;
        }

        public int GetRow()
        {
            return Row;
        }

        public void SetArrayEmpty()
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    _array[i, j] = 0;
                }
            }
        }

        public void SetDiamonds(int n)
        {
            switch (n)
            {
                case 1: // 山
                    _array[0, 1] = 1; _array[1, 0] = 1; _array[1, 1] = 1; _array[1, 2] = 1;
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case 2:
                    _array[0, 0] = 1; _array[1, 0] = 1; _array[1, 1] = 1; _array[1, 2] = 1;
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case 3:
                    _array[0, 2] = 1; _array[1, 0] = 1; _array[1, 1] = 1; _array[1, 2] = 1;
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    _array[0, 0] = 1; _array[0, 1] = 1; _array[1, 1] = 1; _array[1, 2] = 1;
                    break;
                case 5:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    _array[0, 1] = 1; _array[0, 2] = 1; _array[1, 0] = 1; _array[1, 1] = 1;
                    break;
                case 6:          // 方形
                case 7:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    _array[0, 0] = 1; _array[0, 1] = 1; _array[1, 0] = 1; _array[1, 1] = 1;
                    break;
                case 8:          // 长条
                case 9:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    _array[1, 0] = 1; _array[1, 1] = 1; _array[1, 2] = 1; _array[1, 3] = 1;
                    break;
            }

            SevenStyle = n;     // 标记方块的种类

            DrawMapForOutput.RandomNum = new Random().Next(1, 10);      // 提前知道下一个方块是什么

            //          NextDiamonds(DrawMapForOutput.RandomNum , Row, Col);   
        }


        //public static void NextDiamonds(int num, int m, int n)
        //{
        //    int[,] tmpArr = new int[m, n];
        //    switch (num)
        //    {
        //        case 1: // 山
        //            tmpArr[0, 1] = 1; tmpArr[1, 0] = 1; tmpArr[1, 1] = 1; tmpArr[1, 2] = 1;
        //            break;
        //        case 2:
        //            tmpArr[0, 0] = 1; tmpArr[1, 0] = 1; tmpArr[1, 1] = 1; tmpArr[1, 2] = 1;
        //            break;
        //        case 3:
        //            tmpArr[0, 2] = 1; tmpArr[1, 0] = 1; tmpArr[1, 1] = 1; tmpArr[1, 2] = 1;
        //            break;
        //        case 4:
        //            tmpArr[0, 0] = 1; tmpArr[0, 1] = 1; tmpArr[1, 1] = 1; tmpArr[1, 2] = 1;
        //            break;
        //        case 5:
        //            tmpArr[0, 1] = 1; tmpArr[0, 2] = 1; tmpArr[1, 0] = 1; tmpArr[1, 1] = 1;
        //            break;
        //        case 6:          // 方形
        //        case 7:
        //            tmpArr[0, 0] = 1; tmpArr[0, 1] = 1; tmpArr[1, 0] = 1; tmpArr[1, 1] = 1;
        //            break;
        //        case 8:          // 长条
        //        case 9:
        //            tmpArr[1, 0] = 1; tmpArr[1, 1] = 1; tmpArr[1, 2] = 1; tmpArr[1, 3] = 1;
        //            break;
        //    }


        //    int x = 80, y = 10;


        //    for (int i = 0; i < m; i++)
        //    {
        //        Console.SetCursorPosition(x, y);
        //        for (int j = 0; j < n; j++)
        //        {
        //            if (tmpArr[i, j] == 1)
        //                Console.Write("■");
        //            else
        //                Console.Write("  ");
        //        }
        //        Console.WriteLine();
        //        ++y;
        //    }
        //}

    }


    class WelcomeMyFriend
    {
        // 设置光标
        public static Action<int, int> SetCursor = (x, y) => { Console.SetCursorPosition(x, y); };
        public static Action<string> PutText = (str) => { Console.WriteLine(str); };

        public static void SayWelcome()
        {
            var x = Console.BufferWidth;
            SetCursor(x / 2 - 15, 5);
            PutText("< 俄 罗 斯 方 块 >");
            SetCursor(x / 2 - 18, 10);
            PutText("欢 迎 你， 我 的 朋 友 ！");
            SetCursor(0, 23);
            PutText("按两次回车继续 ... ");
        xxx: SetCursor(0, 17);
            PutText("请输入方块下落的速度（单位：ms）：");
            SetCursor(0, 19);
            try
            {
                Diamonds.speed = int.Parse(Console.ReadLine());
            }
            catch (Exception ec)
            {
                PutText(ec.Message);
                goto xxx;
            }

            Console.ReadLine();
            Console.Clear();
        }

        public static void SayExplain()
        {
            SetCursor(70, 5);
            PutText("作者：Fower Dreams");
            SetCursor(70, 7);
            PutText("抱着一颗工匠之心");
            SetCursor(70, 8);
            PutText("想为祖国IT行业作出奉献!");
            SetCursor(70, 9);
            PutText("为爱而战 ...");

            SetCursor(70, 15);
            PutText("游戏规则:");
            SetCursor(80, 17);
            PutText("1. 光标键：→ 、← 控制方块的方向");
            SetCursor(80, 19);
            PutText("2. 光标键：↓ 控制方块的加速");
            SetCursor(80, 21);
            PutText("3. 光标键：↑ 控制方块的变形");
        }
    }
}

