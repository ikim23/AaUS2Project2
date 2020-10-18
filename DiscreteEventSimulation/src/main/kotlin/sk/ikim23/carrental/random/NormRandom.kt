package sk.ikim23.carrental.random

import java.util.*

class NormRandom(val lowerBound: Double, val upperBound: Double, seed: Long = System.currentTimeMillis()) {
    private val rand = Random(seed)

    fun nextDouble(): Double {
        return lowerBound + rand.nextDouble() * (upperBound - lowerBound)
    }

    fun setSeed(seed: Long) = rand.setSeed(seed)
}