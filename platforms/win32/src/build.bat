copy ..\data\*80*.png .
gmcs -target:winexe -out:gbrainy.exe -nowarn:0169 PuzzleFourSided.cs Preferences.cs GtkDialog.cs PlayerHistory.cs PlayerHistoryDialog.cs PuzzleSquaresAndLetters.cs PuzzleTrianglesWithNumbers.cs CalculationFractions.cs PuzzleCountCircles.cs PuzzleEquation.cs PuzzleQuadrilaterals.cs PuzzleCountSeries.cs PuzzleExtraCircle.cs  ArrayListIndicesRandom.cs GameManager.cs PuzzleCirclesRectangle.cs Game.cs PuzzleFigures.cs PuzzleMatrixNumbers.cs PuzzleMoveFigure.cs PuzzlePencil.cs PuzzleSquares.cs PuzzleTriangles.cs PuzzleCoverPercentage.cs PuzzleNumericSequence.cs PuzzleSquareDots.cs PuzzleNumericRelation.cs PuzzleNextFigure.cs PuzzleSquareSheets.cs CalculationArithmetical.cs MemoryColouredFigures.cs GameSession.cs MemoryNumbers.cs Memory.cs MemoryColouredText.cs PuzzleCube.cs MemoryWords.cs PuzzleFigureLetter.cs CustomGameDialog.cs PuzzleDivideCircle.cs CalculationGreatestDivisor.cs CalculationTwoNumbers.cs CalculationWhichNumber.cs PuzzleMatrixGroups.cs PuzzleBalance.cs PuzzleOstracism.cs MemoryCountDots.cs CalculationOperator.cs PuzzleFigurePattern.cs ColorPalette.cs PuzzlePeopleTable.cs GameDrawingArea.cs MemoryFigures.cs PuzzleMissingSlice.cs PuzzleLines.cs PuzzleTetris.cs PreferencesDialog.cs PuzzleMissingPiece.cs MemoryIndications.cs PuzzleMostInCommon.cs PuzzleBuildTriangle.cs CairoContextEx.cs PuzzleClocks.cs gbrainy.cs Defines.cs -pkg:gnome-sharp-2.0 -pkg:gtk-sharp-2.0 -pkg:glade-sharp-2.0 -r:Mono.Cairo.dll -r:Mono.Posix -resource:gbrainy.glade -resource:../data/resume-32.png -resource:../data/endgame-32.png -resource:../data/pause-32.png -resource:../data/allgames-32.png -resource:../data/gbrainy.png -resource:../data/logic-games-32.png -resource:../data/math-games-32.png -resource:../data/memory-games-32.png -resource:../data/gbrainy.svg



