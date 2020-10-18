package sk.ikim23.montecarlo.problem

import sk.ikim23.montecarlo.controllers.XYData
import sk.ikim23.montecarlo.core.IServiceTask
import java.util.*

class KeepGuessTask(val maxReps: Int, val doors: Int) : IServiceTask<XYData> {
    val randGuess = Random()
    val randCar = Random()
    var reps = 0
    var wins = 0.0

    override fun initialize() {
        val rand = Random()
        randGuess.setSeed(rand.nextLong())
        randCar.setSeed(rand.nextLong())
        reps = 0
        wins = 0.0
    }

    override fun hasNext() = reps < maxReps

    override fun tick(): XYData {
        val car = randCar.nextInt(doors)
        val guess = randGuess.nextInt(doors)
        if (car == guess) {
            wins++
        }
        reps++
        return XYData(reps, wins / reps)
    }
}