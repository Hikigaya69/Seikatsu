import { useEffect, useState } from "react";
import api from "../../Utils/api";

import {
  Plus,
  Trash2,
  Package
} from "lucide-react";

export default function Categories() {

  const [categories,setCategories] = useState([]);
  const [newCategory,setNewCategory] = useState("");

  useEffect(()=>{
    fetchCategories();
  },[]);

  const fetchCategories = async()=>{

    try{

      const res =
      await api.get("/Admin/categoryitemcount");

      setCategories(res.data.data);

    }catch(err){

      console.error(err);

    }

  };


  const addCategory = async()=>{

    if(!newCategory.trim()) return;

    try{

      await api.post(
        "/Admin/createcategory",
        JSON.stringify(newCategory),
        {
          headers:{
            "Content-Type":"application/json"
          }
        }
      );

      setNewCategory("");

      fetchCategories();

    }catch(err){

      console.error(err);

    }

  };


  const deleteCategory = async(id)=>{

    try{

      await api.delete(
        `/Admin/deletecategory/${id}`
      );

      fetchCategories();

    }catch(err){

      alert(
        err.response?.data?.message ||
        "Cannot delete category"
      );

    }

  };


  return (

    <div className="space-y-8">

      <div>

        <h1 className="text-3xl font-bold">
          Categories
        </h1>

        <p className="text-gray-500 mt-1">
          Manage product categories
        </p>

      </div>


      <div className="bg-white rounded-2xl shadow p-6 flex gap-4">

        <input
          type="text"
          placeholder="New category..."
          value={newCategory}
          onChange={(e)=>
            setNewCategory(e.target.value)
          }
          className="flex-1 border rounded-lg px-4 py-3"
        />

        <button
          onClick={addCategory}
          className="bg-black text-white px-5 rounded-lg hover:bg-gray-800 transition flex items-center gap-2"
        >

          <Plus size={18}/>
          Add

        </button>

      </div>


      <div className="grid grid-cols-3 gap-6">

        {categories.map(category=>(

          <div
            key={category.id}
            className="bg-white rounded-2xl shadow p-6 hover:shadow-lg transition"
          >

            <div className="flex justify-between items-start">

              <div>

                <h2 className="text-xl font-semibold">
                  {category.categoryName}
                </h2>

                <div className="flex items-center gap-2 text-gray-500 mt-3">

                  <Package size={16}/>

                  <span>
                    {category.productCount} Products
                  </span>

                </div>

              </div>


              <button
                onClick={()=>
                  deleteCategory(category.id)
                }
                className="text-red-500 hover:text-red-700 transition"
              >

                <Trash2 size={18}/>

              </button>

            </div>

          </div>

        ))}

      </div>

    </div>

  );

}