package sk.ikim23.carrental.random

import java.util.*

class ExpRandom(val lambda: Double, seed: Long = System.currentTimeMillis()) {
    private val rand = Random(seed)

    fun nextDouble(): Double {
        return Math.log(1 - rand.nextDouble()) / -lambda
    }

    fun setSeed(seed: Long) = rand.setSeed(seed)
}