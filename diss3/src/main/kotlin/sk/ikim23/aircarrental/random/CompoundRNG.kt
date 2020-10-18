package sk.ikim23.aircarrental.random

import OSPRNG.RNG
import java.util.*

class CompoundRNG(val splitFactor: Double, val left: RNG<Double>, val right: RNG<Double>) : RNG<Double>() {
    private val rand = Random()

    init {
        if (splitFactor !in 0..1) throw IllegalArgumentException()
    }

    override fun sample(): Double {
        val n = rand.nextDouble()
        if (n < splitFactor) {
            return left.sample()
        }
        return right.sample()
    }
}