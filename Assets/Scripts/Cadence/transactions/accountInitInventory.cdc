import BasicInventory from 0x91c7fd71eaa35f2f

transaction {

  prepare(acct: AuthAccount) {}

  execute {
    log(BasicInventory.initializeAccount(_address: 0x01))
  }
}
