require "std"

-- fibonacci numbers tail-recursive method
function tfib(n)
    local function f(a, b, n)
        if n < 3 then
            return b
        else
            return f(b, a+b, n-1)
        end
    end
 
    return f(1, 1, n)
end

print("Value for eleventh number of Fibonacci is " .. tfib(11) .. "\n") -- will print 89

local f = loadfile("info")
f() -- will print extra information