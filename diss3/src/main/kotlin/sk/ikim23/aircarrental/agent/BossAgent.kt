package sk.ikim23.aircarrental.agent

import OSPABA.Simulation
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.M
import sk.ikim23.aircarrental.manager.BossManager
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.ResettableAgent

class BossAgent(mySim: Simulation) : ResettableAgent(ID.AGE_BOSS, mySim, null) {
    init {
        BossManager(mySim, this)
        addOwnMessage(M.INIT)
        addOwnMessage(M.PASSENGER_ARRIVED)
    }

    override fun prepareReplication() {
        super.prepareReplication()
        val initMsg = MyMessage(mySim(), M.INIT, ID.MNG_BOSS)
        manager().notice(initMsg)
    }
}