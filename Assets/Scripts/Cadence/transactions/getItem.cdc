import BasicInventory from 0x91c7fd71eaa35f2f

transaction {

  prepare(acct: AuthAccount) {}

  execute {
    log(BasicInventory.enableOwnership(_address: 0x01, _itemID: 1))
  }
}
