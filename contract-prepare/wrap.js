const Sloffle = require("sloffle")
const main = async () => {
    await Sloffle.wrapping("contract-build", "./ts/contract");
}

main()