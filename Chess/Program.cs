using System;
using System.Collections.Generic;
namespace Chess
{
    enum VALUE
    {
        EMPTY,
        WHITE_KING,
        WHITE_QUEEN,
        WHITE_BISHOP,
        WHITE_KNIGHT,
        WHITE_ROOK,
        WHITE_PAWN,
        BLACK_KING,
        BLACK_QUEEN,
        BLACK_BISHOP,
        BLACK_KNIGHT,
        BLACK_ROOK,
        BLACK_PAWN,
    }
    class Coordinates
    {
        public uint x { get; set; }
        public uint y { get; set; }

        public Coordinates(uint x, uint y)
        {
            this.x = x;
            this.y = y;
        }
    }
    class StepMove
        {
        public Coordinates from { get; set; }
        public Coordinates to { get; set; }
        public StepMove(Coordinates pfrom, Coordinates pto)
        {
            from = pfrom;
            to = pto;
        }
        public StepMove()
        {
        }
    }
    class Board
    {
        private Dictionary<VALUE, string> dict = new Dictionary<VALUE, string>();
        private VALUE[,] desk;
        private List<StepMove> gameHistory;
        private bool isWhiteTurn = true;
        private bool checkBlack = false;
        private bool checkWhite = false;
        private bool whiteKingCheckmate = false;
        private bool blackKingCheckmate = false;
        private bool whiteKingCastling = false;
        private bool blackKingCastling = false;
        private bool enPassant = false;
        private bool WleftRookMove = false;
        private bool WRightRookMove = false;
        private bool BleftRookMove = false;
        private bool BRightRookMove = false;
        private bool WKingMove = false;
        private bool BKingMove = false;
        private Coordinates whiteKingPosition = new Coordinates(0, 0);
        private Coordinates blackKingPosition = new Coordinates(0, 0);
        private bool KingIsSafe(Coordinates position, VALUE[,] chekDesk)  /// !!! Заменил стол на проверочный стол, удалить фром!!
        {
            VALUE kingColor = chekDesk[position.x, position.y];
            if (kingColor == VALUE.WHITE_KING)
            {
                if (chekDesk[position.x - 1, position.y] == VALUE.BLACK_PAWN || chekDesk[position.x + 1, position.y] == VALUE.BLACK_PAWN) return false;//проверка на пешки
                for (int i = (int)position.x + 1; i < 8; i++)
                {
                    if (chekDesk[i, position.y] == VALUE.EMPTY) continue;
                    if (chekDesk[i, position.y] > (VALUE)0 && chekDesk[i, position.y] < (VALUE)7) break;
                    if (chekDesk[i, position.y] != VALUE.BLACK_QUEEN && chekDesk[i, position.y] != VALUE.BLACK_ROOK) break;
                    return false;
                }
                for (int i = (int)position.x - 1; i > -1; i--)
                {
                    if (chekDesk[i, position.y] == VALUE.EMPTY) continue;
                    if (chekDesk[i, position.y] > (VALUE)0 && chekDesk[i, position.y] < (VALUE)7) break;
                    if (chekDesk[i, position.y] != VALUE.BLACK_QUEEN && chekDesk[i, position.y] != VALUE.BLACK_ROOK) break;
                    return false;
                }
                for (int i = (int)position.y + 1; i < 8; i++)
                {
                    if (chekDesk[position.x, i] == VALUE.EMPTY) continue;
                    if (chekDesk[position.x, i] > (VALUE)0 && chekDesk[position.x, i] < (VALUE)7) break;
                    if (chekDesk[position.x, i] != VALUE.BLACK_QUEEN && chekDesk[position.x, i] != VALUE.BLACK_ROOK) break;
                    return false;
                }
                for (int i = (int)position.y - 1; i > -1; i--)
                {
                    if (chekDesk[position.x, i] == VALUE.EMPTY) continue;
                    if (chekDesk[position.x, i] > (VALUE)0 && chekDesk[position.x, i] < (VALUE)7) break;
                    if (chekDesk[position.x, i] != VALUE.BLACK_QUEEN && chekDesk[position.x, i] != VALUE.BLACK_ROOK) break;
                    return false;
                }
                for (int x = (int)position.x + 1, y = (int)position.y + 1; x > -1 && x < 8 && y > -1 && y < 8; x++, y++)
                {
                    if (chekDesk[x, y] == VALUE.EMPTY) continue;
                    if (chekDesk[x, y] > (VALUE)0 && chekDesk[x, y] < (VALUE)7) break;
                    if (chekDesk[x, y] != VALUE.BLACK_QUEEN && chekDesk[x, y] != VALUE.BLACK_BISHOP) break;
                    return false;
                }
                for (int x = (int)position.x + 1, y = (int)position.y - 1; x > -1 && x < 8 && y > -1 && y < 8; x++, y--)
                {
                    if (chekDesk[x, y] == VALUE.EMPTY) continue;
                    if (chekDesk[x, y] > (VALUE)0 && chekDesk[x, y] < (VALUE)7) break;
                    if (chekDesk[x, y] != VALUE.BLACK_QUEEN && chekDesk[x, y] != VALUE.BLACK_BISHOP) break;
                    return false;
                }
                for (int x = (int)position.x - 1, y = (int)position.y + 1; x > -1 && x < 8 && y > -1 && y < 8; x--, y++)
                {
                    if (chekDesk[x, y] == VALUE.EMPTY) continue;
                    if (chekDesk[x, y] > (VALUE)0 && chekDesk[x, y] < (VALUE)7) break;
                    if (chekDesk[x, y] != VALUE.BLACK_QUEEN && chekDesk[x, y] != VALUE.BLACK_BISHOP) break;
                    return false;
                }
                for (int x = (int)position.x - 1, y = (int)position.y - 1; x > -1 && x < 8 && y > -1 && y < 8; x--, y--)
                {
                    if (chekDesk[x, y] == VALUE.EMPTY) continue;
                    if (chekDesk[x, y] > (VALUE)0 && chekDesk[x, y] < (VALUE)7) break;
                    if (chekDesk[x, y] != VALUE.BLACK_QUEEN && chekDesk[x, y] != VALUE.BLACK_BISHOP) break;
                    return false;
                }

                if ((int)position.x + 1 < 8 && (int)position.y + 2 < 8)
                {
                    if (chekDesk[position.x + 1, position.y + 2] == VALUE.BLACK_KNIGHT) return false;
                }
                if ((int)position.x + 2 < 8 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x + 2, position.y + 1] == VALUE.BLACK_KNIGHT) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y + 2 < 8)
                {
                    if (chekDesk[position.x - 1, position.y + 2] == VALUE.BLACK_KNIGHT) return false;
                }
                if ((int)position.x - 2 > -1 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x - 2, position.y + 1] == VALUE.BLACK_KNIGHT) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y - 2 > -1)
                {
                    if (chekDesk[position.x - 1, position.y - 1] == VALUE.BLACK_KNIGHT) return false;
                }
                if ((int)position.x - 1 > -2 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x - 1, position.y - 1] == VALUE.BLACK_KNIGHT) return false;
                }
                if ((int)position.x + 1 < 8 && (int)position.y - 2 > -1)
                {
                    if (chekDesk[position.x + 1, position.y - 2] == VALUE.BLACK_KNIGHT) return false;
                }
                if ((int)position.x + 2 < 8 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x + 2, position.y - 1] == VALUE.BLACK_KNIGHT) return false;
                }


                if ((int)position.x + 1 < 8 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x + 1, position.y + 1] == VALUE.BLACK_KING) return false;
                }
                if ((int)position.x + 1 < 8 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x + 1, position.y + 1] == VALUE.BLACK_KING) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x - 1, position.y + 1] == VALUE.BLACK_KING) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x - 1, position.y + 1] == VALUE.BLACK_KING) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x - 1, position.y - 1] == VALUE.BLACK_KING) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x - 1, position.y - 1] == VALUE.BLACK_KING) return false;
                }
                if ((int)position.x + 1 < 8 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x + 1, position.y - 1] == VALUE.BLACK_KING) return false;
                }
                if ((int)position.x + 1 < 8 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x + 1, position.y - 1] == VALUE.BLACK_KING) return false;
                }
                return true;
            }
            else if (kingColor == VALUE.BLACK_KING)
            {
                if (chekDesk[position.x - 1, position.y] == VALUE.WHITE_PAWN || chekDesk[position.x + 1, position.y] == VALUE.WHITE_PAWN) return false;//проверка на пешки
                for (int i = (int)position.x + 1; i < 8; i++)
                {
                    if (chekDesk[i, position.y] == VALUE.EMPTY) continue;
                    if (chekDesk[i, position.y] > (VALUE)6) break;
                    if (chekDesk[i, position.y] != VALUE.WHITE_QUEEN && chekDesk[i, position.y] != VALUE.WHITE_ROOK) break;
                    return false;
                }
                for (int i = (int)position.x - 1; i > -1; i--)
                {
                    if (chekDesk[i, position.y] == VALUE.EMPTY) continue;
                    if (chekDesk[i, position.y] > (VALUE)6) break;
                    if (chekDesk[i, position.y] != VALUE.WHITE_QUEEN && chekDesk[i, position.y] != VALUE.WHITE_ROOK) break;
                    return false;
                }
                for (int i = (int)position.y + 1; i < 8; i++)
                {
                    if (chekDesk[position.x, i] == VALUE.EMPTY) continue;
                    if (chekDesk[position.x, i] > (VALUE)6) break;
                    if (chekDesk[position.x, i] != VALUE.WHITE_QUEEN && chekDesk[position.x, i] != VALUE.WHITE_ROOK) break;
                    return false;
                }
                for (int i = (int)position.y - 1; i > -1; i--)
                {
                    if (chekDesk[position.x, i] == VALUE.EMPTY) continue;
                    if (chekDesk[position.x, i] > (VALUE)6) break;
                    if (chekDesk[position.x, i] != VALUE.WHITE_QUEEN && chekDesk[position.x, i] != VALUE.WHITE_ROOK) break;
                    return false;
                }
                for (int x = (int)position.x + 1, y = (int)position.y + 1; x > -1 && x < 8 && y > -1 && y < 8; x++, y++)
                {
                    if (chekDesk[x, y] == VALUE.EMPTY) continue;
                    if (chekDesk[x, y] > (VALUE)6) break;
                    if (chekDesk[x, y] != VALUE.WHITE_QUEEN && chekDesk[x, y] != VALUE.WHITE_BISHOP) break;
                    return false;
                }
                for (int x = (int)position.x + 1, y = (int)position.y - 1; x > -1 && x < 8 && y > -1 && y < 8; x++, y--)
                {
                    if (chekDesk[x, y] == VALUE.EMPTY) continue;
                    if (chekDesk[x, y] > (VALUE)6) break;
                    if (chekDesk[x, y] != VALUE.WHITE_QUEEN && chekDesk[x, y] != VALUE.WHITE_BISHOP) break;
                    return false;
                }
                for (int x = (int)position.x - 1, y = (int)position.y + 1; x > -1 && x < 8 && y > -1 && y < 8; x--, y++)
                {
                    if (chekDesk[x, y] == VALUE.EMPTY) continue;
                    if (chekDesk[x, y] > (VALUE)6) break;
                    if (chekDesk[x, y] != VALUE.WHITE_QUEEN && chekDesk[x, y] != VALUE.WHITE_BISHOP) break;
                    return false;
                }
                for (int x = (int)position.x - 1, y = (int)position.y - 1; x > -1 && x < 8 && y > -1 && y < 8; x--, y--)
                {
                    if (chekDesk[x, y] == VALUE.EMPTY) continue;
                    if (chekDesk[x, y] > (VALUE)6) break;
                    if (chekDesk[x, y] != VALUE.WHITE_QUEEN && chekDesk[x, y] != VALUE.WHITE_BISHOP) break;
                    return false;
                }

                if ((int)position.x + 1 < 8 && (int)position.y + 2 < 8)
                {
                    if (chekDesk[position.x + 1, position.y + 2] == VALUE.WHITE_KNIGHT) return false;
                }
                if ((int)position.x + 2 < 8 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x + 2, position.y + 1] == VALUE.WHITE_KNIGHT) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y + 2 < 8)
                {
                    if (chekDesk[position.x - 1, position.y + 2] == VALUE.WHITE_KNIGHT) return false;
                }
                if ((int)position.x - 2 > -1 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x - 2, position.y + 1] == VALUE.WHITE_KNIGHT) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y - 2 > -1)
                {
                    if (chekDesk[position.x - 1, position.y - 1] == VALUE.WHITE_KNIGHT) return false;
                }
                if ((int)position.x - 1 > -2 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x - 1, position.y - 1] == VALUE.WHITE_KNIGHT) return false;
                }
                if ((int)position.x + 1 < 8 && (int)position.y - 2 > -1)
                {
                    if (chekDesk[position.x + 1, position.y - 2] == VALUE.WHITE_KNIGHT) return false;
                }
                if ((int)position.x + 2 < 8 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x + 2, position.y - 1] == VALUE.WHITE_KNIGHT) return false;
                }


                if ((int)position.x + 1 < 8 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x + 1, position.y + 1] == VALUE.WHITE_KING) return false;
                }
                if ((int)position.x + 1 < 8 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x + 1, position.y + 1] == VALUE.WHITE_KING) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x - 1, position.y + 1] == VALUE.WHITE_KING) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y + 1 < 8)
                {
                    if (chekDesk[position.x - 1, position.y + 1] == VALUE.WHITE_KING) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x - 1, position.y - 1] == VALUE.WHITE_KING) return false;
                }
                if ((int)position.x - 1 > -1 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x - 1, position.y - 1] == VALUE.WHITE_KING) return false;
                }
                if ((int)position.x + 1 < 8 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x + 1, position.y - 1] == VALUE.WHITE_KING) return false;
                }
                if ((int)position.x + 1 < 8 && (int)position.y - 1 > -1)
                {
                    if (chekDesk[position.x + 1, position.y - 1] == VALUE.WHITE_KING) return false;
                }
                return true;
            }
            else
            {
                return false;//исключение
            }
        }
        private bool IsCheckmate()
        {
            if (!isWhiteTurn)
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        if (desk[x, y] < (VALUE)7 && desk[x, y] > 0) continue;
                        for (int to_y = 0; to_y < 8; to_y++)
                        {
                            for (int to_x = 0; to_x < 8; to_x++)
                            {
                                //fromTo.to = new Coordinates((uint)x, (uint)y);
                                //fromTo.to = new Coordinates((uint)to_x, (uint)to_y);
                                StepMove fromTo = new StepMove(new Coordinates((uint)x, (uint)y), new Coordinates((uint)to_x, (uint)to_y));
                                if (CanIMove(fromTo))
                                {
                                    return false; //
                                }
                            }
                        }
                    }

                }
                return true;
            }
            else //if (isWhiteTurn) 7 6 - 4 4
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        if (desk[x, y] > (VALUE)6) continue;
                        for (int to_y = 0; to_y < 8; to_y++)
                        {
                            for (int to_x = 0; to_x < 8; to_x++)
                            {
                                StepMove fromTo = new StepMove(new Coordinates((uint)x, (uint)y), new Coordinates((uint)to_x, (uint)to_y));
                                if (CanIMove(fromTo))
                                {
                                    return false; //
                                }
                            }
                        }
                    }

                }
                return true;
            }
        }
        private StepMove NotationToIndx(string notation)
        {
            try
            {
                Coordinates from = new Coordinates(Convert.ToUInt16(Convert.ToChar(notation[0]) - 'a'), UInt16.Parse(notation.Substring(1, 1)) - (uint)1);
                Coordinates to = new Coordinates(Convert.ToUInt16(Convert.ToChar(notation[3]) - 'a'), UInt16.Parse(notation.Substring(4, 1)) - (uint)1);
                StepMove fromTo = new StepMove(from,to);
                return fromTo;
            }
            catch
            {
                Coordinates to = new Coordinates(0, 0);
                Coordinates from = new Coordinates(0, 0);
                StepMove fromTo = new StepMove(from, to);
                return fromTo;
            }
        }
        private bool ShadowMovePiece(StepMove fromTo, VALUE[,] chekDesk)
        {
            Coordinates from = fromTo.from;
            Coordinates to = fromTo.to;
            chekDesk[to.x, to.y] = chekDesk[from.x, from.y];
            chekDesk[from.x, from.y] = VALUE.EMPTY;
            return true;
        }
        private bool CanIMove(StepMove fromTo)
        {
            Coordinates from = fromTo.from;
            Coordinates to = fromTo.to;
            if (from.y > 7 || from.y < 0 || from.x > 7 || from.x < 0 ||
                to.y > 7 || to.y < 0 || to.x > 7 || to.x < 0)
            {
                return false;

            }
            VALUE first_piece = desk[from.x, from.y];
            VALUE second_piece = desk[to.x, to.y];
            if (first_piece > (VALUE)6 && isWhiteTurn)
            {
                return false;
            }
            else if (first_piece < (VALUE)7 && !isWhiteTurn)
            {
                return false;
            }
            VALUE[,] chekDesk = (VALUE[,])desk.Clone();
            ShadowMovePiece(fromTo, chekDesk);
            int dx = (int)(to.x - from.x);
            int dy = (int)(to.y - from.y);
            int sx = 0;
            if (dx != 0) sx = dx / Math.Abs(dx);
            int sy = 0;
            if (dy != 0) sy = dy / Math.Abs(dy);
            switch (first_piece)
            {
                case VALUE.WHITE_KNIGHT:
                    if ((Math.Abs(dx) != 1 || Math.Abs(dy) != 2) && (Math.Abs(dx) != 2 || Math.Abs(dy) != 1)) { return false; }
                    if ((uint)second_piece > 6 || (uint)second_piece == 0)
                    {
                        return KingIsSafe(whiteKingPosition, chekDesk);
                    }
                    else
                    {
                        return false;
                    }
                case VALUE.BLACK_KNIGHT:
                    if ((Math.Abs(dx) != 1 || Math.Abs(dy) != 2) && (Math.Abs(dx) != 2 || Math.Abs(dy) != 1)) { return false; }
                    if ((uint)second_piece < 7)
                    {
                        return KingIsSafe(blackKingPosition, chekDesk);
                    }
                    else
                    {
                        return false;
                    }
                case VALUE.WHITE_ROOK:
                    if ((uint)second_piece > 0 && (uint)second_piece < 7) { return false; }
                    if (dy == 0)
                    {
                        for (int i = (int)from.x + sx; i != to.x; i += sx)
                        {
                            if (desk[i, from.y] == VALUE.EMPTY)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return KingIsSafe(whiteKingPosition, chekDesk);
                    }
                    else if (dx == 0)
                    {
                        for (int i = (int)from.y + sy; i != to.y; i += sy)
                        {
                            if (desk[from.x, i] == VALUE.EMPTY)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return KingIsSafe(whiteKingPosition, chekDesk);
                    }
                    else
                    {
                        return false;
                    }
                case VALUE.BLACK_ROOK:
                    if ((uint)second_piece > 6) { return false; }
                    if (dy == 0)
                    {
                        for (int i = (int)from.x + sx; i != to.x; i += sx)
                        {
                            if (desk[i, from.y] == VALUE.EMPTY)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return KingIsSafe(blackKingPosition, chekDesk);
                    }
                    else if (dx == 0)
                    {
                        for (int i = (int)from.y + sy; i != to.y; i += sy)
                        {
                            if (desk[from.x, i] == VALUE.EMPTY)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return KingIsSafe(blackKingPosition, chekDesk);
                    }
                    else
                    {
                        return false;
                    }
                case VALUE.WHITE_BISHOP:
                    if ((uint)second_piece > 0 && (uint)second_piece < 7) { return false; }
                    if (Math.Abs(dx) != Math.Abs(dy)) { return false; }
                    for (int x = (int)from.x + sx, y = (int)from.y + sy; x != to.x; x += sx, y += sy)
                    {
                        if (desk[x, y] == VALUE.EMPTY)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return KingIsSafe(whiteKingPosition, chekDesk);
                case VALUE.BLACK_BISHOP:
                    if ((uint)second_piece > 6) { return false; }
                    if (Math.Abs(dx) != Math.Abs(dy)) { return false; }
                    for (int x = (int)from.x + sx, y = (int)from.y + sy; x != to.x; x += sx, y += sy)
                    {
                        if (desk[x, y] == VALUE.EMPTY)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return true;
                case VALUE.WHITE_PAWN:
                    if ((uint)second_piece > 0 && (uint)second_piece < 7) { return false; }
                    if (from.x == to.x)
                    {
                        if (to.y - from.y == 1)// ход на одну клетку вперед

                        {
                            return KingIsSafe(whiteKingPosition, chekDesk);
                        }
                        else if (to.y - from.y == 2 && from.y == 1 && desk[from.x, from.y + 1] == VALUE.EMPTY)//ход на две клеки вперед
                        {
                            return KingIsSafe(whiteKingPosition, chekDesk);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (Math.Abs(dx) == 1 && dy == 1 && second_piece != VALUE.EMPTY)//рубим на искосок
                    {
                        return KingIsSafe(whiteKingPosition, chekDesk);
                    }
                    else if (Math.Abs(dx) == 1 && dy == 1 && from.y == 4 && desk[to.x, to.y - 1] == (VALUE)12) //взятие на проходе

                    {
                        StepMove lastMove = gameHistory[gameHistory.Count - 1];
                        if (lastMove.from.x == to.x && lastMove.from.y == 6 && lastMove.to.x == to.x && lastMove.to.y == to.y-1)
                        {
                            enPassant = true;
                            return KingIsSafe(whiteKingPosition, chekDesk);         
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                case VALUE.BLACK_PAWN:
                    if ((uint)second_piece > 6) { return false; }
                    if (from.x == to.x)
                    {
                        if (dy == -1)// ход на одну клетку вперед

                        {
                            return KingIsSafe(blackKingPosition, chekDesk);
                        }
                        else if (dy == -2 && from.y == 6 && desk[from.x, from.y - 1] == VALUE.EMPTY)//ход на две клеки вперед
                        {
                            return KingIsSafe(blackKingPosition, chekDesk);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (Math.Abs(dx) == 1 && dy == -1 && second_piece != VALUE.EMPTY)//рубим на искосок
                    {
                        return KingIsSafe(blackKingPosition, chekDesk);
                    }
                    else if (Math.Abs(dx) == 1 && dy == -1 && from.y == 3 && desk[to.x, to.y + 1] > (VALUE)0 && desk[to.x, to.y + 1] < (VALUE)7) //взятие на проходе
                    {
                        StepMove lastMove = gameHistory[gameHistory.Count - 1];
                        if (lastMove.from.x == to.x && lastMove.from.y == 1 && lastMove.to.x == to.x && lastMove.to.y == to.y+1)
                        {
                            enPassant = true;
                            return KingIsSafe(whiteKingPosition, chekDesk);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    
                    
                case VALUE.WHITE_QUEEN:
                    if ((uint)second_piece > 0 && (uint)second_piece < 7) { return false; }
                    if (Math.Abs(dx) == Math.Abs(dy))
                    {
                        for (int x = (int)from.x + sx, y = (int)from.y + sy; x != to.x; x += sx, y += sy)
                        {
                            if (desk[x, y] == VALUE.EMPTY)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return KingIsSafe(whiteKingPosition, chekDesk);
                    }
                    else if (dy == 0)
                    {
                        for (int i = (int)from.x + sx; i != to.x; i += sx)
                        {
                            if (desk[i, from.y] == VALUE.EMPTY)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return KingIsSafe(whiteKingPosition, chekDesk);
                    }
                    else if (dx == 0)
                    {
                        for (int i = (int)from.y + sy; i != to.y; i += sy)
                        {
                            if (desk[from.x, i] == VALUE.EMPTY)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return KingIsSafe(whiteKingPosition, chekDesk);
                    }
                    else
                    {
                        return false;
                    }
                case VALUE.BLACK_QUEEN:
                    if ((uint)second_piece > 6) { return false; }
                    if (Math.Abs(dx) == Math.Abs(dy))
                    {
                        for (int x = (int)from.x + sx, y = (int)from.y + sy; x != to.x; x += sx, y += sy)
                        {
                            if (desk[x, y] == VALUE.EMPTY)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return KingIsSafe(blackKingPosition, chekDesk);
                    }
                    else if (dy == 0)
                    {
                        for (int i = (int)from.x + sx; i != to.x; i += sx)
                        {
                            if (desk[i, from.y] == VALUE.EMPTY)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return KingIsSafe(blackKingPosition, chekDesk);
                    }
                    else if (dx == 0)
                    {
                        for (int i = (int)from.y + sy; i != to.y; i += sy)
                        {
                            if (desk[from.x, i] == VALUE.EMPTY)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return KingIsSafe(blackKingPosition, chekDesk);
                    }
                    else
                    {
                        return false;
                    }
                case VALUE.WHITE_KING:
                    if ((uint)second_piece > 0 && (uint)second_piece < 7) { return false; }
                    if (Math.Abs(dx) < 2 && Math.Abs(dy) < 2)
                    {
                        return KingIsSafe(to, chekDesk);
                    }
                    else
                    {
                        if (!WKingMove && !checkWhite)
                        {
                            if(to.x == from.x-2 && !WleftRookMove)
                            { 
                                if (desk[from.x - 1, from.y] != VALUE.EMPTY || desk[from.x-2,from.y]!=VALUE.EMPTY || desk[from.x-3, to.y] != VALUE.EMPTY) return false; //клетки заняты

                                chekDesk = (VALUE[,])desk.Clone();
                                Coordinates toCastlingSell1 = new Coordinates(from.x - 1, from.y);
                                ShadowMovePiece(new StepMove(from, toCastlingSell1), chekDesk);
                                
                                if (!KingIsSafe(toCastlingSell1, chekDesk)) return false; //клетка под ударом

                                Coordinates toCastlingSell2 = new Coordinates(from.x - 2, from.y);


                                ShadowMovePiece(new StepMove(toCastlingSell1, toCastlingSell2), chekDesk);
                                if (!KingIsSafe(toCastlingSell2, chekDesk)) return false; //клетка под ударом

                                whiteKingCastling = true;
                                WRightRookMove = true;
                                return true;

                            }
                            else if(to.x == from.x+2 && !WRightRookMove)
                            {
                                chekDesk = (VALUE[,])desk.Clone();
                                if (desk[from.x + 1, from.y] != VALUE.EMPTY || desk[from.x + 2, from.y] != VALUE.EMPTY) return false; //клетки заняты

                                Coordinates toCastlingSell1 = new Coordinates(from.x + 1, from.y);
                                ShadowMovePiece(new StepMove(from, toCastlingSell1), chekDesk);

                                if (!KingIsSafe(toCastlingSell1, chekDesk)) return false; //клетка под ударом

                                Coordinates toCastlingSell2 = new Coordinates(from.x + 2, from.y);  


                                ShadowMovePiece(new StepMove(toCastlingSell1, toCastlingSell2), chekDesk);
                                if (!KingIsSafe(toCastlingSell2, chekDesk)) return false; //клетка под ударом

                                whiteKingCastling = true;
                                WleftRookMove = true;
                                return true;
                            }
                        }

                    }
                    return false;
                case VALUE.BLACK_KING:
                    if ((uint)second_piece > 6) { return false; }
                    if (Math.Abs(dx) < 2 && Math.Abs(dy) < 2)
                    {
                        return KingIsSafe(to, chekDesk);    
                    }
                    else
                    {
                        if (!BKingMove && !checkBlack)
                        {
                            if (to.x == from.x - 2 && !BleftRookMove)
                            {
                                if (desk[from.x - 1, from.y] != VALUE.EMPTY || desk[from.x - 2, from.y] != VALUE.EMPTY || desk[from.x - 3, from.y] != VALUE.EMPTY) return false; //клетки заняты
                                chekDesk = (VALUE[,])desk.Clone();
                                Coordinates toCastlingSell1 = new Coordinates(from.x - 1, from.y);
                                ShadowMovePiece(new StepMove(from, toCastlingSell1), chekDesk);

                                if (!KingIsSafe(toCastlingSell1, chekDesk)) return false; //клетка под ударом

                                Coordinates toCastlingSell2 = new Coordinates(from.x - 2, from.y);


                                ShadowMovePiece(new StepMove(toCastlingSell1, toCastlingSell2), chekDesk);
                                if (!KingIsSafe(toCastlingSell2, chekDesk)) return false; //клетка под ударом

                                blackKingCastling = true;
                                BRightRookMove = true;
                                return true;

                            }
                            else if (to.x == from.x + 2 && !BRightRookMove)
                            {
                                if (desk[from.x + 1, from.y] != VALUE.EMPTY || desk[from.x + 2, from.y] != VALUE.EMPTY) return false; //клетки заняты
                                chekDesk = (VALUE[,])desk.Clone();
                                Coordinates toCastlingSell1 = new Coordinates(from.x + 1, from.y);
                                ShadowMovePiece(new StepMove(from, toCastlingSell1), chekDesk);

                                if (!KingIsSafe(toCastlingSell1, chekDesk)) return false; //клетка под ударом

                                Coordinates toCastlingSell2 = new Coordinates(from.x + 2, from.y);


                                ShadowMovePiece(new StepMove(toCastlingSell1, toCastlingSell2), chekDesk);
                                if (!KingIsSafe(toCastlingSell2, chekDesk)) return false; //клетка под ударом

                                blackKingCastling = true;
                                BleftRookMove = true;
                                return true;
                            }
                        }

                    }

                    return false;
                case VALUE.EMPTY:
                    return false;
                default:
                    return false;
            }
        }
        public bool MovePiece(string notation)
        {

            StepMove fromTo = NotationToIndx(notation);
            Coordinates from = fromTo.from;
            Coordinates to = fromTo.to;
            if (CanIMove(fromTo))
            {
                desk[to.x, to.y] = desk[from.x, from.y];
                desk[from.x, from.y] = VALUE.EMPTY;
                if (desk[to.x, to.y] == VALUE.BLACK_KING) blackKingPosition = to;
                if (desk[to.x, to.y] == VALUE.WHITE_KING) whiteKingPosition = to;
                if (enPassant)
                {
                    StepMove lastFromTo = gameHistory[gameHistory.Count - 1];
                    desk[lastFromTo.to.x, lastFromTo.to.y] = VALUE.EMPTY;
                    enPassant = false;
                }
                if (whiteKingCastling)
                {
                    if (WRightRookMove)
                    {
                        desk[to.x - 2, to.y] = VALUE.EMPTY;
                        desk[to.x + 1, to.y] = VALUE.WHITE_ROOK;
                        
                    }
                    else{
                        desk[to.x + 1, to.y] = VALUE.EMPTY;
                        desk[to.x - 1, to.y] = VALUE.WHITE_ROOK;
                    }
                    WKingMove = true;
                    whiteKingCastling = false;
                }
                if (blackKingCastling)
                {
                    if (WRightRookMove)
                    {
                        desk[to.x - 2, to.y] = VALUE.EMPTY;
                        desk[to.x + 1, to.y] = VALUE.WHITE_ROOK;
                    }
                    else
                    {
                        desk[to.x + 1, to.y] = VALUE.EMPTY;
                        desk[to.x - 1, to.y] = VALUE.WHITE_ROOK;
                    }
                    BKingMove = true;
                    blackKingCastling = false;
                }
                isWhiteTurn = !isWhiteTurn;
                gameHistory.Add(fromTo);
                if (!KingIsSafe(blackKingPosition, desk))
                {
                    checkBlack = true;
                    if (IsCheckmate())
                    {
                        blackKingCheckmate = true;
                    }
                }
                else
                {
                    checkBlack = false;
                }
                if (!KingIsSafe(whiteKingPosition, desk))
                {
                    checkWhite = true;
                    if (IsCheckmate())
                        whiteKingCheckmate = true;
                }
                else
                {
                    checkWhite = false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public void DrawDesk()
        {
            for (int y = 7; y > -1; y--)
            {
                Console.Write('\n');
                for (int x = 0; x < 8; x++)
                {
                    Console.Write(dict[desk[x, y]]);
                }
                Console.Write(y+1);
                Console.Write('\n');
            }
            Console.WriteLine(" a   b   c   d   e   f   g   h");
            if (isWhiteTurn) { Console.WriteLine("WHITE TURN"); } else { Console.WriteLine("BLACK TURN"); }
            if (blackKingCheckmate)
            {
                Console.WriteLine("CHECKMATE BLACK");
                Console.WriteLine("GAME OVER");

            }
            else if (checkBlack) { 
                Console.WriteLine("CHECK BLACK");
            }

            if (whiteKingCheckmate)
            {
                Console.WriteLine("CHECKMATE WHITE");
                Console.WriteLine("GAME OVER");

            }
            else if (checkWhite)
            {
                Console.WriteLine("CHECK WHITE");
            }
        }
        public Board()
        {
            desk = new VALUE[8, 8];
            gameHistory = new List<StepMove>();
            for (int x = 0; x < 8; x++)
            {
                desk[x, 1] = VALUE.WHITE_PAWN;
                desk[x, 6] = VALUE.BLACK_PAWN;
            }
            desk[0, 0] = VALUE.WHITE_ROOK;
            desk[7, 0] = VALUE.WHITE_ROOK;
            desk[0, 7] = VALUE.BLACK_ROOK;
            desk[7, 7] = VALUE.BLACK_ROOK;
            desk[1, 0] = VALUE.WHITE_KNIGHT;
            desk[6, 0] = VALUE.WHITE_KNIGHT;
            desk[1, 7] = VALUE.BLACK_KNIGHT;
            desk[6, 7] = VALUE.BLACK_KNIGHT;
            desk[2, 0] = VALUE.WHITE_BISHOP;
            desk[5, 0] = VALUE.WHITE_BISHOP;
            desk[2, 7] = VALUE.BLACK_BISHOP;
            desk[5, 7] = VALUE.BLACK_BISHOP;
            desk[3, 0] = VALUE.WHITE_QUEEN;
            desk[4, 0] = VALUE.WHITE_KING;
            desk[3, 7] = VALUE.BLACK_QUEEN;
            desk[4, 7] = VALUE.BLACK_KING;
            whiteKingPosition.x = 4;
            whiteKingPosition.y = 0;
            blackKingPosition.x = 4;
            blackKingPosition.y = 7;
            dict.Add(VALUE.EMPTY, "[  ]");
            dict.Add(VALUE.WHITE_KING, "[♔ ]");
            dict.Add(VALUE.BLACK_KING, "[♚ ]");
            dict.Add(VALUE.WHITE_QUEEN, "[♕ ]");
            dict.Add(VALUE.BLACK_QUEEN, "[♛ ]");
            dict.Add(VALUE.WHITE_ROOK, "[♖ ]");
            dict.Add(VALUE.BLACK_ROOK, "[♜ ]");
            dict.Add(VALUE.WHITE_BISHOP, "[♗ ]");
            dict.Add(VALUE.BLACK_BISHOP, "[♝ ]");
            dict.Add(VALUE.WHITE_KNIGHT, "[♘ ]");
            dict.Add(VALUE.BLACK_KNIGHT, "[♞ ]");
            dict.Add(VALUE.WHITE_PAWN, "[♙ ]");
            dict.Add(VALUE.BLACK_PAWN, "[♟]");
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Board myBoard = new Board();
            string inputNotation;
            myBoard.DrawDesk();
            while (true)
            {
                inputNotation = Console.ReadLine();
                if (inputNotation == "exit") return 0;
                if (inputNotation == "new game")
                {
                    myBoard = new Board();
                    myBoard.DrawDesk();
                    inputNotation = Console.ReadLine();
                }
                if (!myBoard.MovePiece(inputNotation))
                {
                    Console.Clear();
                    myBoard.DrawDesk();
                    Console.WriteLine("INCORRECT INPUT");
                }
                else
                {
                    Console.Clear();
                    myBoard.DrawDesk();
                }
            }

        }
    }
}
