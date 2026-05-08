import { useEffect, useState } from "react";

import api from "../../Utils/api";

export default function Inventory() {

  const [products,setProducts] = useState([]);

  useEffect(()=>{

    fetchProducts();

  },[]);


  const fetchProducts = async()=>{

    try{

      const res =
      await api.get("/Admin/adminfilter");

      setProducts(res.data.data.items);

    }catch(err){

      console.error(err);

    }

  };


  return (

    <div className="space-y-8">

      <div>

        <h1 className="text-4xl font-bold">
          Inventory Hub
        </h1>

        <p className="text-gray-500 mt-2">
          Product stock monitoring
        </p>

      </div>


      <div className="bg-white rounded-2xl border shadow-sm p-6">

        <table className="w-full">

          <thead>

            <tr className="border-b text-left text-gray-500">

              <th className="pb-4">
                Product
              </th>

              <th className="pb-4">
                Category
              </th>

              <th className="pb-4">
                Price
              </th>

              <th className="pb-4">
                Status
              </th>

            </tr>

          </thead>


          <tbody>

            {products.map(product=>(

              <tr
                key={product.id}
                className="border-b"
              >

                <td className="py-5">

                  <div className="flex items-center gap-4">

                    <img
                      src={product.productImageUrl}
                      className="w-16 h-16 rounded-xl object-cover"
                    />

                    <span className="font-medium">
                      {product.name}
                    </span>

                  </div>

                </td>


                <td>
                  {product.category}
                </td>

                <td>
                  ₹{product.price}
                </td>

                <td>

                  <span className="bg-green-100 text-green-700 px-3 py-1 rounded-full text-sm">

                    Active

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