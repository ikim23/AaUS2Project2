package sk.ikim23.aircarrental.random

import java.util.*

class SplitFactor(val factor: Double, val value: Int)

class NumberRNG(val splitFactors: List<SplitFactor>) {
    private val rand = Random()

    fun sample(): Int {
        val n = rand.nextDouble()
        for (sf in splitFactors) {
            if (n < sf.factor) {
                return sf.value
            }
        }
        return -1
    }
}