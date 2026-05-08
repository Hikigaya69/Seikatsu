const restocks = [

  {
    id:1,
    product:"Sushi Rice",
    frequency:"Weekly",
    nextOrder:"Tomorrow",
    status:"Active"
  },

  {
    id:2,
    product:"Mochi Pack",
    frequency:"Monthly",
    nextOrder:"May 12",
    status:"Paused"
  },

  {
    id:3,
    product:"Matcha Kit",
    frequency:"Biweekly",
    nextOrder:"Friday",
    status:"Active"
  }

];

export default function RestockManagement() {

  return (

    <div className="space-y-8">

      <div>

        <h1 className="text-4xl font-bold">
          Restock Management
        </h1>

        <p className="text-gray-500 mt-2">
          Auto-order subscription monitoring
        </p>

      </div>


      <div className="grid grid-cols-4 gap-6">

        <div className="bg-white rounded-2xl p-6 border shadow-sm">

          <p className="text-gray-500">
            Active Plans
          </p>

          <h2 className="text-3xl font-bold mt-3">
            128
          </h2>

        </div>


        <div className="bg-white rounded-2xl p-6 border shadow-sm">

          <p className="text-gray-500">
            Paused Plans
          </p>

          <h2 className="text-3xl font-bold mt-3">
            12
          </h2>

        </div>


        <div className="bg-white rounded-2xl p-6 border shadow-sm">

          <p className="text-gray-500">
            Upcoming Orders
          </p>

          <h2 className="text-3xl font-bold mt-3">
            34
          </h2>

        </div>


        <div className="bg-white rounded-2xl p-6 border shadow-sm">

          <p className="text-gray-500">
            Monthly Revenue
          </p>

          <h2 className="text-3xl font-bold mt-3">
            ?280k
          </h2>

        </div>

      </div>


      <div className="bg-white rounded-2xl border shadow-sm p-6">

        <h2 className="text-2xl font-semibold mb-6">
          Scheduled Orders
        </h2>


        <table className="w-full">

          <thead>

            <tr className="border-b text-left text-gray-500">

              <th className="pb-4">
                Product
              </th>

              <th className="pb-4">
                Frequency
              </th>

              <th className="pb-4">
                Next Order
              </th>

              <th className="pb-4">
                Status
              </th>

            </tr>

          </thead>


          <tbody>

            {restocks.map(item=>(

              <tr
                key={item.id}
                className="border-b"
              >

                <td className="py-5 font-medium">

                  {item.product}

                </td>


                <td>

                  {item.frequency}

                </td>


                <td>

                  {item.nextOrder}

                </td>


                <td>

                  <span
                    className={`px-3 py-1 rounded-full text-sm ${
                      item.status==="Paused"
                      ? "bg-yellow-100 text-yellow-700"
                      : "bg-green-100 text-green-700"
                    }`}
                  >

                    {item.status}

                  </span>

                </td>

              </tr>

            ))}

          </tbody>

        </table>

      </div>

    </div>

  );

}