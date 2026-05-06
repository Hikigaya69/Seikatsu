import api from "../../Utils/api";

import {
  Trash2,
  Pencil
} from "lucide-react";

export default function ProductTable({
  data,
  refresh
}) {

  const deleteProduct = async(id)=>{

    try{

      await api.delete(
        `/Admin/deleteproduct/${id}`
      );

      refresh();

    }catch(err){
      console.error(err);
    }

  };

  return (

    <div className="bg-white rounded-2xl shadow overflow-hidden">

      <div className="grid grid-cols-6 bg-gray-100 p-4 font-semibold text-sm">

        <p>Product</p>
        <p>Category</p>
        <p>Country</p>
        <p>Price</p>
        <p>Created</p>
        <p>Actions</p>

      </div>


      {data.map(product=>(

        <div
          key={product.id}
          className="grid grid-cols-6 items-center p-4 border-b hover:bg-gray-50 transition"
        >

          <div className="flex items-center gap-3">

            <img
              src={product.productImageUrl}
              alt={product.name}
              className="w-14 h-14 rounded-xl object-cover"
            />

            <div>

              <p className="font-medium">
                {product.name}
              </p>

              <p className="text-xs text-gray-400">
                {product.description?.slice(0,40)}
              </p>

            </div>

          </div>


          <p>{product.category}</p>

          <p>{product.countryName}</p>

          <p>₹{product.price}</p>

          <p className="text-sm text-gray-500">
            {new Date(product.createdAt)
              .toLocaleDateString()}
          </p>


          <div className="flex gap-4">

            <button className="text-gray-500 hover:text-black">

              <Pencil size={18}/>

            </button>

            <button
              onClick={()=>deleteProduct(product.id)}
              className="text-red-500 hover:text-red-700"
            >

              <Trash2 size={18}/>

            </button>

          </div>

        </div>

      ))}

    </div>

  );

}