import { useState } from "react";

import api from "../../Utils/api";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from "@/components/ui/dialog";

import { Button } from "@/components/ui/button";

import { Input } from "@/components/ui/input";

export default function ProductDialog({
  categories,
  refresh
}) {

  const [preview,setPreview] = useState("");

  const [form,setForm] = useState({

    name:"",
    description:"",
    price:"",
    countryName:"",
    storageType:"",
    categoryId:"",
    isFood:true,
    image:null

  });

  const handleImage = (e)=>{

    const file = e.target.files[0];

    setForm({
      ...form,
      image:file
    });

    setPreview(
      URL.createObjectURL(file)
    );

  };

  const handleSubmit = async()=>{

    try{

      const formData = new FormData();

      formData.append("Name",form.name);
      formData.append("Description",form.description);
      formData.append("Price",form.price);
      formData.append("CountryName",form.countryName);
      formData.append("StorageType",form.storageType);
      formData.append("CategoryId",form.categoryId);
      formData.append("IsFood",form.isFood);
      formData.append("Image",form.image);

      await api.post(
        "/Admin/adminaddproducts",
        formData
      );

      refresh();

    }catch(err){
      console.error(err);
    }

  };

  return (

    <Dialog>

      <DialogTrigger asChild>

        <Button>
          Add Product
        </Button>

      </DialogTrigger>


      <DialogContent className="max-w-2xl">

        <DialogHeader>

          <DialogTitle>
            Add New Product
          </DialogTitle>

        </DialogHeader>


        <div className="grid grid-cols-2 gap-5">

          <Input
            placeholder="Product name"
            onChange={(e)=>
              setForm({
                ...form,
                name:e.target.value
              })
            }
          />

          <Input
            placeholder="Price"
            type="number"
            onChange={(e)=>
              setForm({
                ...form,
                price:e.target.value
              })
            }
          />

          <Input
            placeholder="Country"
            onChange={(e)=>
              setForm({
                ...form,
                countryName:e.target.value
              })
            }
          />

          <Input
            placeholder="Storage Type"
            onChange={(e)=>
              setForm({
                ...form,
                storageType:e.target.value
              })
            }
          />


          <select
            className="border rounded-lg px-3 h-10"
            onChange={(e)=>
              setForm({
                ...form,
                categoryId:e.target.value
              })
            }
          >

            <option>
              Select Category
            </option>

            {categories.map(category=>(
              <option
                key={category.id}
                value={category.id}
              >
                {category.categoryName}
              </option>
            ))}

          </select>


          <input
            type="file"
            onChange={handleImage}
          />

        </div>


        <textarea
          placeholder="Description"
          className="w-full border rounded-lg p-3 mt-4"
          rows={4}
          onChange={(e)=>
            setForm({
              ...form,
              description:e.target.value
            })
          }
        />


        {preview &&(

          <img
            src={preview}
            className="w-40 h-40 object-cover rounded-xl mt-4"
          />

        )}


        <Button
          onClick={handleSubmit}
          className="w-full mt-5"
        >
          Save Product
        </Button>

      </DialogContent>

    </Dialog>

  );

}