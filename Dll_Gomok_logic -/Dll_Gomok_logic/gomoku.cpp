#include "pch.h"
#include "gomoku.h"
#include <iostream>
#include <cstring>
#include <algorithm>
#include <vector>
#include <deque>
#include <windows.h>

using namespace std;

const int MAX_DEPTH = 5; // 탐색 깊이 설정

int map[19][19]; // 바둑 판
int w_board[19][19]; // 가중치 판
int dir[8][2] = { { 1,0 },{ 0,1 },{ 1,1 },{ 1,-1 },{ -1,0 },{ 0,-1 },{ -1,-1 },{ -1,1 } };

int n = 19;
int w[2][6] = { { 0,1,50,9999,500000,10000000 },{ 0,1,12,250,400000,10000000 } };
int w2[2][6][3][2];
int stx, sty;
int ansx, ansy;
int real_high[6];

typedef struct xy {
    int x, y;
} xy;

typedef struct info {
    int num = 0, enemy = 0, emptyspace = 0;
} info;

typedef struct info2 {
    int x, y, weight;
};

bool cmp(info2 a, info2 b) {
    return a.weight > b.weight;
}

void add_weight(int color[2]);
void search(int cnt, int color);
int minimax(int depth, int color, int alpha, int beta, bool maximizingPlayer);
void AI(int user_color, int ai_color);
void input(int type);
void game_type(int type);
bool check(int color);
void init();
void init_weights();

void init() {
    memset(map, 0, sizeof(map));
    memset(w_board, 0, sizeof(w_board));
    init_weights();
}

void init_weights() {
    w2[0][1][0][0] = 2; w2[1][1][0][0] = 1;
    w2[0][1][0][1] = 2; w2[1][1][0][1] = 0;
    w2[0][2][0][0] = 25, w2[1][2][0][0] = 4;
    w2[0][2][0][1] = 25, w2[1][2][0][1] = 1;
    w2[0][2][1][1] = 2; w2[1][2][1][1] = 1;
    w2[0][2][1][0] = 2; w2[1][2][1][0] = 1;
    w2[0][3][0][0] = 521, w2[1][3][0][0] = 105;
    w2[0][3][0][1] = 301; w2[1][3][0][1] = 13;
    w2[0][3][1][0] = 301, w2[1][3][1][0] = 13;
    w2[0][3][1][1] = 301, w2[1][3][1][1] = 13;
    w2[0][4][0][0] = 21000; w2[0][4][1][0] = 20010; w2[0][4][2][0] = 20010;
    w2[1][4][0][0] = 4001; w2[1][4][1][0] = 4001; w2[1][4][2][0] = 4001;
}

void add_weight(int color[2]) {
    memset(w_board, 0, sizeof(w_board));

    for (int type = 0; type < 2; type++) {
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                int sum = 0;
                info Count[5];
                if (map[i][j]) continue;
                for (int d = 0; d < 4; d++) {
                    int nx, ny;
                    int cnt = 1;
                    int zerocnt = 0;
                    int zerocnt1 = 0;
                    int remember = 0;
                    int zerocnt2 = 0;
                    int num = 0;
                    int enemy_cnt = 0;
                    int before;

                    while (true) {
                        nx = i + (cnt * dir[d][0]), ny = j + (cnt * dir[d][1]);
                        before = map[nx - dir[d][0]][ny - dir[d][1]];
                        if (nx < 0 || ny < 0 || nx >= n || ny >= n) {
                            if (remember || zerocnt1 == 0) {
                                enemy_cnt++;
                            }
                            if (before != 0) remember = zerocnt1;
                            break;
                        }
                        if (map[nx][ny] == color[(type + 1) % 2]) {
                            if (remember || zerocnt1 == 0) {
                                enemy_cnt++;
                            }
                            if (before != 0) remember = zerocnt1;
                            break;
                        }

                        if (map[nx][ny] == color[type]) {
                            remember = zerocnt1;
                            num++;
                        }
                        if (map[nx][ny] == 0) zerocnt1++;
                        if (zerocnt1 >= 2) break;
                        cnt++;
                    }
                    zerocnt1 = remember;
                    cnt = 1;
                    remember = 0;

                    while (true) {
                        nx = i + (cnt * dir[d + 4][0]), ny = j + (cnt * dir[d + 4][1]);
                        if (nx < 0 || ny < 0 || nx >= n || ny >= n) {
                            if (remember || zerocnt2 == 0) {
                                enemy_cnt++;
                            }
                            if (before != 0) remember = zerocnt2;
                            break;
                        }
                        if (map[nx][ny] == color[(type + 1) % 2]) {
                            if (remember || zerocnt2 == 0) {
                                enemy_cnt++;
                            }
                            if (before != 0) remember = zerocnt2;
                            break;
                        }

                        if (map[nx][ny] == color[type]) {
                            remember = zerocnt2;
                            num++;
                        }
                        if (map[nx][ny] == 0) zerocnt2++;
                        if (zerocnt2 >= 2) break;
                        cnt++;
                    }
                    zerocnt2 = remember;
                    zerocnt = zerocnt1 + zerocnt2;
                    Count[d] = { num, enemy_cnt, zerocnt };
                }

                for (int d = 0; d < 4; d++) {
                    int num = Count[d].num, enemy = Count[d].enemy, emptyspace = Count[d].emptyspace;
                    int temp_w = w2[(type + 1) % 2][num][enemy][emptyspace];

                    if (emptyspace >= 2 || num + emptyspace >= 5) continue;
                    if (num != 4) {
                        sum += temp_w;
                    }
                    else {
                        sum += temp_w;
                    }
                }
                w_board[i][j] += sum;
            }
        }
    }
}

void search(int cnt, int color) {
    memset(real_high, 0, sizeof(real_high));
    int color_arr[2] = { color, 3 - color };

    add_weight(color_arr);

    vector<info2> v;
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            if (!map[i][j]) {
                v.push_back({ i, j, w_board[i][j] });
            }
        }
    }
    sort(v.begin(), v.end(), cmp);

    stx = v[0].x;
    sty = v[0].y;
}

int minimax(int depth, int color, int alpha, int beta, bool maximizingPlayer) {
    if (depth == MAX_DEPTH) {
        return w_board[stx][sty]; // 평가 함수 호출 또는 현재 보드의 상태를 바탕으로 값을 반환
    }

    int bestValue;
    if (maximizingPlayer) {
        bestValue = INT_MIN;
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                if (!map[i][j]) {
                    map[i][j] = color;
                    int value = minimax(depth + 1, 3 - color, alpha, beta, false);
                    map[i][j] = 0;
                    bestValue = max(bestValue, value);
                    alpha = max(alpha, bestValue);
                    if (beta <= alpha) {
                        break;
                    }
                }
            }
        }
        return bestValue;
    }
    else {
        bestValue = INT_MAX;
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                if (!map[i][j]) {
                    map[i][j] = color;
                    int value = minimax(depth + 1, 3 - color, alpha, beta, true);
                    map[i][j] = 0;
                    bestValue = min(bestValue, value);
                    beta = min(beta, bestValue);
                    if (beta <= alpha) {
                        break;
                    }
                }
            }
        }
        return bestValue;
    }
}

void AI(int user_color, int ai_color) {
    if (map[9][9] == 0) {
        ansx = 9;
        ansy = 9;
        map[ansx][ansy] = ai_color;
        return;
    }

    int bestValue = INT_MIN;
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            if (!map[i][j]) {
                map[i][j] = ai_color;
                int value = minimax(0, user_color, INT_MIN, INT_MAX, false); // 미니맥스 호출
                map[i][j] = 0;
                if (value > bestValue) {
                    bestValue = value;
                    ansx = i;
                    ansy = j;
                }
            }
        }
    }
    map[ansx][ansy] = ai_color;
}


bool check(int color) {
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            if (map[i][j] == color) {
                for (int d = 0; d < 4; d++) {
                    int cnt = 1;
                    for (int k = 1; k < 5; k++) {
                        int nx = i + k * dir[d][0];
                        int ny = j + k * dir[d][1];
                        if (nx >= 0 && ny >= 0 && nx < n && ny < n && map[nx][ny] == color) cnt++;
                        else break;
                    }
                    if (cnt >= 5) return true;
                }
            }
        }
    }
    return false;
}

extern "C" {
    GOMOKU_API void InitGame() {
        init();
    }

    GOMOKU_API void InputPosition(int x, int y, int player) {
        map[x][y] = player;
    }

    GOMOKU_API void GetNextMove(int player, int* x, int* y) {
        AI(3 - player, player);
        *x = ansx;
        *y = ansy;
    }

    GOMOKU_API bool CheckWin(int player) {
        return check(player);
    }

    GOMOKU_API void PrintBoard() {
        for (int i = 0; i < 19; i++) {
            for (int j = 0; j < 19; j++) {
                std::cout << map[i][j] << " ";
            }
            std::cout << std::endl;
        }
    }

    GOMOKU_API bool IsPositionEmpty(int x, int y) {
        return map[x][y] == 0;
    }
}
