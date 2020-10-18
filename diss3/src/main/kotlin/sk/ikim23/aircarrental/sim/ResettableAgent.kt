package sk.ikim23.aircarrental.sim

import OSPABA.Agent
import OSPABA.Simulation

open class ResettableAgent(id: Int, mySim: Simulation, parent: Agent?) : Agent(id, mySim, parent), IResettable {
    override fun reset() {
        children().forEach { if(it is IResettable) it.reset() }
    }
}