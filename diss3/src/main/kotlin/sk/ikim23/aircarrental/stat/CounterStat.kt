package sk.ikim23.aircarrental.stat

import OSPABA.Simulation
import sk.ikim23.aircarrental.sim.Config

class CounterStat(private val mySim: Simulation) {
    private var count = 0

    fun inc(value: Int = 1): Int {
        if (mySim.currentTime() > Config.warmTime) {
            count += value
        }
        return count
    }

    fun count(): Int = count

    fun clear() {
        count = 0
    }
}