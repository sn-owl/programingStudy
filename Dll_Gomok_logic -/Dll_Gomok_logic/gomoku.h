#pragma once

#ifdef GOMOKU_EXPORTS
#define GOMOKU_API __declspec(dllexport)
#else
#define GOMOKU_API __declspec(dllimport)
#endif

extern "C" {
    GOMOKU_API void InitGame();
    GOMOKU_API void InputPosition(int x, int y, int player);
    GOMOKU_API void GetNextMove(int player, int* x, int* y);
    GOMOKU_API bool CheckWin(int player);
    GOMOKU_API void PrintBoard();
}
