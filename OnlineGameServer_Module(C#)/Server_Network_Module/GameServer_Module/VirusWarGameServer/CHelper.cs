﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirusWarGameServer
{
    // 게임 로직 구현 시 도움이 되는 메소드를 모아 놓은 클래스
    class CHelper
    {
        static byte COLUMN_COUNT = 7;

        // 포지션을 (row, col)형식의 좌표로 변환한다.
        static Vector2 convert_to_xy(short position)
        {
            return new Vector2(calc_row(position), calc_col(position));
        }

        // (row, col)형식의 좌표를 포지션으로 변환한다.
        public static short get_position(byte row, byte col)
        {
            return (short)(row * COLUMN_COUNT + col);
        }

        // 포지션으로부터 세로 인덱스를 구한다.
        public static short calc_row(short position)
        {
            return (short)(position / COLUMN_COUNT);
        }

        // 포지션으로부터 가로 인덱스를 구한다.
        public static short calc_col(short position)
        {
            return (short)(position % COLUMN_COUNT);
        }

        // cell 인덱스를 넣으면 둘 사이의 거리값을 리턴해 준다.
        // 한칸이 차이나면 1, 두칸이 차이나면 2
        public static short get_distance(short from, short to)
        {
            Vector2 pos1 = convert_to_xy(from);
            Vector2 pos2 = convert_to_xy(to);

            return get_distance(pos1, pos2);
        }

        public static short get_distance(Vector2 pos1, Vector2 pos2)
        {
            Vector2 distance = pos1 - pos2;

            short x = (short)Math.Abs(distance.x);
            short y = (short)Math.Abs(distance.y);

            // x,y중 큰 값이 실제 두 위치 사이의 거리를 뜻한다.
            return Math.Max(x, y);
        }

        public static byte howfar_from_clicked_cell(short basis_cell, short cell)
        {
            short row = (short)(basis_cell / COLUMN_COUNT);
            short col = (short)(basis_cell % COLUMN_COUNT);

            Vector2 basis_pos = new Vector2(col, row);

            row = (short)(cell / COLUMN_COUNT);
            col = (short)(cell % COLUMN_COUNT);
            Vector2 cell_pos = new Vector2(col, row);

            Vector2 distnace = (basis_pos - cell_pos);

            short x = (short)Math.Abs(distnace.x);
            short y = (short)Math.Abs(distnace.y);

            return (byte)Math.Max(x, y);
        }

        // 주위에 있는 셀의 위치를 찾아서 리스트로 리턴해 준다.
        public static List<short> find_neighbor_cells(short basis_cell, List<short> targets, short gap)
        {
            Vector2 pos = convert_to_xy(basis_cell);

            return targets.FindAll(obj => get_distance(pos, convert_to_xy(obj)) <= gap);
        }

        // 게임을 지속할 수 있는지 체크한다.
        public static bool can_play_more(List<short> board, CPlayer current_player, List<CPlayer> all_player)
        {
            foreach (short cell in current_player.viruses)
            {
                if (CHelper.find_available_cells(cell, board, all_player).Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // 이동 가능한 셀을 찾아서 리스트로 반환한다.
        public static List<short> find_available_cells(short basis_cell, List<short> total_cells, List<CPlayer> players)
        {
            List<short> targets = find_neighbor_cells(basis_cell, total_cells, 2);

            players.ForEach(obj =>
            {
                targets.RemoveAll(number => obj.viruses.Exists(cell => cell == number));
            });

            return targets;
        }
    }
}