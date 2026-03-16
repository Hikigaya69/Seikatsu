import { ShoppingCart, User } from "lucide-react";

export default function HomeNavbar() {
  return (
    <div className="bg-white border-b p-2 shadow-sm">

      <div className="max-w-7xl mx-auto flex items-center justify-between p-5">

        
        <h1 className="text-2xl font-bold text-[#284b63]">
          Seikatsu
        </h1>

        
        <input
          type="text"
          placeholder="Search product"
          className="w-125 px-4 py-2 border rounded-lg"
        />

        
        <div className="flex gap-6">

          <User className="cursor-pointer" />

          <ShoppingCart className="cursor-pointer" />

        </div>

      </div>

    </div>
  );
}