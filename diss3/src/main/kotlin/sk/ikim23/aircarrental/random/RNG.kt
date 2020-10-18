package sk.ikim23.aircarrental.random

import java.util.*

class Piece(endTime: Number, mean: Number) {
    val endTime = endTime.toDouble()
    val mean = mean.toDouble()
}

data class IndexedPiece(val index: Int, val endTime: Double, val mean: Double)

class Rand(pieces: List<Piece>) {
    private val pieces = pieces.mapIndexed { i, p -> IndexedPiece(i, p.endTime, p.mean) }
    private val rand = Random()

    fun exponential(mean: Double): Double {
        return -Math.log(1.0 - rand.nextDouble()) * mean
    }

    fun sample(currentTime: Double): Double {
        var time = currentTime
        var piece = actualPiece(time)
        var s = exponential(piece.mean)
        while (time + s >= piece.endTime) {
            if (piece == pieces.last()) {
                break
            }
            s = (s - (piece.endTime - time)) * pieces[piece.index + 1].mean / piece.mean
            time = piece.endTime
            piece = pieces[piece.index + 1]
        }
        return time + s - currentTime
    }

    private fun actualPiece(currentTime: Double): IndexedPiece {
        for (p in pieces) {
            if (currentTime < p.endTime) {
                return p
            }
        }
        return pieces.last()
    }
}