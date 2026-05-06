import { useEffect, useState } from "react";
import api from "../../Utils/api";

import ProductTable from "../../components/admin/ProductTable";
import ProductDialog from "../../components/admin/ProductDialog";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

export default function Products() {

  const [products,setProducts] = useState([]);
  const [categories,setCategories] = useState([]);

  const [search,setSearch] = useState("");
  const [selectedCategory,setSelectedCategory] = useState("");

  useEffect(()=>{
    fetchProducts();
    fetchCategories();
  },[]);

  const fetchProducts = async()=>{

    try{

      const res = await api.get(
        "/Admin/adminfilter?pageSize=20"
      );

      setProducts(res.data.data.items);

    }catch(err){
      console.error(err);
    }

  };

  const fetchCategories = async()=>{

    try{

      const res = await api.get(
        "/Admin/getallcats"
      );

      setCategories(res.data.data);

    }catch(err){
      console.error(err);
    }

  };

  const filteredProducts = products.filter(product=>{

    const matchesSearch =
      product.name.toLowerCase()
      .includes(search.toLowerCase());

    const matchesCategory =
      !selectedCategory ||
      product.category === selectedCategory;

    return matchesSearch && matchesCategory;

  });

  return (

    <div className="space-y-6">

      <div className="flex justify-between items-center">

        <div>

          <h1 className="text-3xl font-bold">
            Products
          </h1>

          <p className="text-gray-500 mt-1">
            Manage store inventory
          </p>

        </div>

        <ProductDialog
          categories={categories}
          refresh={fetchProducts}
        />

      </div>


      <div className="flex gap-4">

        <Input
          placeholder="Search product..."
          value={search}
          onChange={(e)=>setSearch(e.target.value)}
          className="max-w-sm"
        />

        <select
          value={selectedCategory}
          onChange={(e)=>
            setSelectedCategory(e.target.value)
          }
          className="border rounded-lg px-3"
        >

          <option value="">
            All Categories
          </option>

          {categories.map(category=>(
            <option
              key={category.id}
              value={category.categoryName}
            >
              {category.categoryName}
            </option>
          ))}

        </select>

      </div>


      <ProductTable
        data={filteredProducts}
        refresh={fetchProducts}
      />

    </div>

  );

}